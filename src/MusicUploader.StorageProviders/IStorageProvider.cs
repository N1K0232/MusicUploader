namespace MusicUploader.StorageProviders;

public interface IStorageProvider
{
    Task DeleteAsync(string path);

    Task<Stream> ReadAsync(string path);

    Task UploadAsync(string path, Stream stream);
}