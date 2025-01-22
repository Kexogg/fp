namespace TagsCloudContainerCore.Facade;

public interface ITagCloud
{
    Result<byte[]> FromFile(string filePath);
    Result<byte[]> FromString(string data);
    Result<byte[]> FromBytes(byte[] data);
}