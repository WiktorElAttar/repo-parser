using System.IO.Compression;
using System.Net;
using NSubstitute;
using RepoParser.Slices.Github.Http;
using RepoParser.Slices.Github.Services;

namespace RepoParser.Slices.GitHub.Tests;

public class GitHubServiceTests
{
    private readonly IGitHubHttpClient _gitHubClient = Substitute.For<IGitHubHttpClient>();
    private readonly GitHubService _sut;

    public GitHubServiceTests()
    {
        _sut = new GitHubService(_gitHubClient);
    }

    [Fact]
    public async Task GetStatistics_WithValidJsFile_ReturnsCorrectLetterCounts()
    {
        // Arrange
        var repository = "test/repo";
        var jsContent = "fuNction heLlo() { return 'abc'; }";
        var zipStream = CreateZipStreamWithFiles(new Dictionary<string, string>
        {
            { "file.js", jsContent },
        });
            
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StreamContent(zipStream)
        };

        _gitHubClient.GetArchive(repository, Arg.Any<CancellationToken>())
            .Returns(httpResponse);

        // Act
        var result = await _sut.GetStatistics(repository, CancellationToken.None);

        // Assert
        AssertDictionaryEntry(result, 'u', 2);
        AssertDictionaryEntry(result, 'c', 2);
        AssertDictionaryEntry(result, 't', 2);
        AssertDictionaryEntry(result, 'o', 2);
        AssertDictionaryEntry(result, 'n', 2);
        AssertDictionaryEntry(result, 'e', 2);
        AssertDictionaryEntry(result, 'r', 2);
        AssertDictionaryEntry(result, 'f', 1);
        AssertDictionaryEntry(result, 'N', 1);
        AssertDictionaryEntry(result, 'i', 1);
        AssertDictionaryEntry(result, 'h', 1);
        AssertDictionaryEntry(result, 'L', 1);
        AssertDictionaryEntry(result, 'l', 1);
        AssertDictionaryEntry(result, 'a', 1);
        AssertDictionaryEntry(result, 'b', 1);
        Assert.Equal(15, result.Count);
    }

    [Fact]
    public async Task GetStatistics_WithMultipleFiles_OnlyProcessesJsAndTsFiles()
    {
        // Arrange
        var repository = "test/repo";
        var jsContent = "function hello() { return 'abc'; }";
        var tsContent = "fuNction heLlo(prefix: string): string { return prefix +'ABC'; }";
        var txtContent = "123aBC";

        var zipStream = CreateZipStreamWithFiles(new Dictionary<string, string>
        {
            { "file.js", jsContent },
            { "file.ts", tsContent },
            { "file.txt", txtContent }
        });

        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StreamContent(zipStream)
        };

        _gitHubClient.GetArchive(repository, Arg.Any<CancellationToken>())
            .Returns(httpResponse);

        // Act
        var result = await _sut.GetStatistics(repository, CancellationToken.None);

        // Assert
        AssertDictionaryEntry(result, 'r', 8);
        AssertDictionaryEntry(result, 'n', 7);
        AssertDictionaryEntry(result, 't', 6);
        AssertDictionaryEntry(result, 'i', 6);
        AssertDictionaryEntry(result, 'e', 6);
        AssertDictionaryEntry(result, 'f', 4);
        AssertDictionaryEntry(result, 'u', 4);
        AssertDictionaryEntry(result, 'o', 4);
        AssertDictionaryEntry(result, 'c', 3);
        AssertDictionaryEntry(result, 'l', 3);
        AssertDictionaryEntry(result, 'h', 2);
        AssertDictionaryEntry(result, 'p', 2);
        AssertDictionaryEntry(result, 'x', 2);
        AssertDictionaryEntry(result, 's', 2);
        AssertDictionaryEntry(result, 'g', 2);
        AssertDictionaryEntry(result, 'N', 1);
        AssertDictionaryEntry(result, 'L', 1);
        AssertDictionaryEntry(result, 'A', 1);
        AssertDictionaryEntry(result, 'B', 1);
        AssertDictionaryEntry(result, 'C', 1);
        AssertDictionaryEntry(result, 'a', 1);
        AssertDictionaryEntry(result, 'b', 1);
        Assert.Equal(22, result.Count);
    }

    [Fact]
    public async Task GetStatistics_WithHttpFailure_ThrowsException()
    {
        // Arrange
        var repository = "test/repo";
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        };

        _gitHubClient.GetArchive(repository, Arg.Any<CancellationToken>())
            .Returns(httpResponse);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _sut.GetStatistics(repository, CancellationToken.None));
    }

    private static Stream CreateZipStreamWithFiles(Dictionary<string, string> files)
    {
        var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var file in files)
            {
                var entry = archive.CreateEntry(file.Key);
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream);
                writer.Write(file.Value);
            }
        }
            
        memoryStream.Position = 0;
        return memoryStream;
    }

    private static void AssertDictionaryEntry(Dictionary<char, int> result, char letter, int expectedCount)
    {
        Assert.True(result.ContainsKey(letter));
        Assert.Equal(expectedCount, result[letter]);
    }
}