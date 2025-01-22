namespace TagsCloudContainerCore.Models.Graphics;

public struct Color
{
    private readonly uint _color;
    
    public Color(byte r, byte g, byte b)
    {
        _color = 0xff000000u | (uint)(r << 16) | (uint)(g << 8) | b;
    }

    public Color(string hex)
    {
        if (hex.StartsWith("#"))
        {
            hex = hex[1..];
        }
        
        _color = hex.Length switch
        {
            6 => 0xff000000u | (uint)Convert.ToInt32(hex, 16),
            3 => (0xff000000u | (uint)Convert.ToInt32(new string(hex.SelectMany(c => new[] {c, c}).ToArray()), 16)),
            _ => throw new ArgumentException("Hex color code should be 3 or 6 characters long.")
        };
    }
    
    public uint ToUint() => _color;
}