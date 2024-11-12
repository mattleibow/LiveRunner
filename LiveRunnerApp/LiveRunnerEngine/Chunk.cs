namespace LiveRunnerEngine;

public record class Chunk
{
    public string? Name { get; init; }

    public required int Length { get; init; }

    public required int Lanes { get; init; }

    public required SpriteKind[][] Sprites { get; init; }
}
