using System;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using GameLibraryApi;
using GameLibraryApi.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameLibraryApi.Json.UnitTests;

[TestClass]
public partial class DateOnlyJsonConverterTests
{
    /// <summary>
    /// Verifies that Read returns default(DateOnly) when the reader's token value is null or an empty JSON string.
    /// Inputs tested:
    ///  - JSON literal null (expects reader.GetString() == null)
    ///  - JSON empty string literal "" (expects reader.GetString() == string.Empty)
    /// Expected result: method returns default(System.DateOnly) for both cases and does not throw.
    /// </summary>
    [TestMethod]
    public void Read_NullOrEmptyString_ReturnsDefault()
    {
        // Arrange
        var converter = new DateOnlyJsonConverter();
        var options = new JsonSerializerOptions();

        string[] jsonInputs = new[]
        {
                "null",     // JSON null literal -> GetString() returns null
                "\"\""      // JSON empty string -> GetString() returns string.Empty
            };

        foreach (var json in jsonInputs)
        {
            // Act
            var reader = CreateReaderAndRead(json);
            var result = converter.Read(ref reader, typeof(DateOnly), options);

            // Assert
            Assert.AreEqual(default(DateOnly), result, $"Expected default(DateOnly) for JSON: {json}");
        }
    }

    /// <summary>
    /// Verifies that Read correctly parses a well-formed date string into a DateOnly value.
    /// Input: JSON string "2021-12-31"
    /// Expected: Returns DateOnly representing 2021-12-31.
    /// </summary>
    [TestMethod]
    public void Read_ValidDateString_ReturnsParsedDate()
    {
        // Arrange
        var converter = new DateOnlyJsonConverter();
        var options = new JsonSerializerOptions();
        const string json = "\"2021-12-31\"";

        // Act
        var reader = CreateReaderAndRead(json);
        var result = converter.Read(ref reader, typeof(DateOnly), options);

        // Assert
        var expected = DateOnly.Parse("2021-12-31");
        Assert.AreEqual(expected, result);
    }

    /// <summary>
    /// Helper that creates a Utf8JsonReader for the provided JSON payload and advances it to the first token.
    /// Returns the reader (struct) ready to be passed by ref to the converter.
    /// </summary>
    /// <param name="json">A JSON payload representing a single JSON token (e.g., "\"2021-12-31\"", "null").</param>
    /// <returns>Utf8JsonReader positioned on the token corresponding to the payload.</returns>
    private static Utf8JsonReader CreateReaderAndRead(string json)
    {
        var bytes = Encoding.UTF8.GetBytes(json);
        var reader = new Utf8JsonReader(bytes);
        // Move to the first token (string, null, etc.)
        Assert.IsTrue(reader.Read(), "Failed to read the JSON token for input: " + json);
        return reader;
    }

    /// <summary>
    /// Verifies that DateOnlyJsonConverter.Write produces the expected JSON string value
    /// for a variety of DateOnly inputs including boundary and formatting edge-cases.
    /// Inputs tested: DateOnly.MinValue, DateOnly.MaxValue, a leap day, single-digit month/day,
    /// and a typical end-of-year date. Expected: each call writes a JSON string with the
    /// exact format "yyyy-MM-dd" (zero-padded year/month/day) and no extra characters.
    /// </summary>
    [TestMethod]
    public void Write_DateOnlyValues_WritesCorrectIsoDateString()
    {
        // Arrange
        var converter = new DateOnlyJsonConverter();
        var options = new JsonSerializerOptions();

        var testCases = new (DateOnly Value, string ExpectedJson)[]
        {
                (DateOnly.MinValue, "\"0001-01-01\""),
                (DateOnly.MaxValue, "\"9999-12-31\""),
                (new DateOnly(2000, 2, 29), "\"2000-02-29\""), // leap day
                (new DateOnly(2021, 3, 5), "\"2021-03-05\""),  // single-digit month/day -> zero padded
                (new DateOnly(2023, 12, 31), "\"2023-12-31\"") // typical date
        };

        foreach (var (value, expectedJson) in testCases)
        {
            // Use a fresh in-memory buffer for each case to isolate writes.
            var buffer = new ArrayBufferWriter<byte>();
            using var writer = new Utf8JsonWriter(buffer);

            // Act
            converter.Write(writer, value, options);
            writer.Flush();

            // Assert
            var writtenBytes = buffer.WrittenMemory.ToArray();
            var json = Encoding.UTF8.GetString(writtenBytes);
            Assert.AreEqual(expectedJson, json, $"Value {value} produced unexpected JSON.");
        }
    }

    /// <summary>
    /// Ensures that consecutive Write calls append JSON string values to the writer
    /// in sequence (writer is not reset by the converter). This verifies writer usage
    /// behavior: multiple writes should be present in the output in call order.
    /// Inputs: two distinct DateOnly values. Expected: concatenated JSON string values.
    /// </summary>
    [TestMethod]
    public void Write_MultipleCalls_AppendsValuesInSequence()
    {
        // Arrange
        var converter = new DateOnlyJsonConverter();
        var options = new JsonSerializerOptions();

        var value1 = new DateOnly(2022, 1, 2);
        var value2 = new DateOnly(1999, 12, 31);

        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);

        // Act
        writer.WriteStartArray();
        converter.Write(writer, value1, options);
        converter.Write(writer, value2, options);
        writer.WriteEndArray();
        writer.Flush();

        // Assert
        var writtenBytes = buffer.WrittenMemory.ToArray();
        var json = Encoding.UTF8.GetString(writtenBytes);
        // Expect two JSON string tokens inside an array: "["2022-01-02","1999-12-31"]"
        var expected = $"[\"{value1:yyyy-MM-dd}\",\"{value2:yyyy-MM-dd}\"]";
        Assert.AreEqual(expected, json);
    }
}