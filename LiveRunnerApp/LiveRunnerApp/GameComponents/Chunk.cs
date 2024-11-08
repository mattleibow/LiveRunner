namespace LiveRunnerApp.GameComponents;

public record class Chunk
{
    public required int Length { get; set; } 

    public required int Lanes { get; set; } 

    public required SpriteKind[][] Sprites { get; init; }
}
