using SK = LiveRunnerEngine.SpriteKind;

namespace LiveRunnerEngine;

public static class Chunks
{
    public static readonly Chunk Empty =
        new()
        {
            Name = nameof(Empty),
            Length = 5,
            Lanes = 3,
            Sprites =
            [
                [SK.None, SK.None, SK.None, SK.None, SK.None],
                [SK.None, SK.None, SK.None, SK.None, SK.None],
                [SK.None, SK.None, SK.None, SK.None, SK.None],
            ]
        };

    public static readonly Chunk RightLaneBlockedWithObstacle =
        new()
        {
            Name = nameof(RightLaneBlockedWithObstacle),
            Length = 5,
            Lanes = 3,
            Sprites =
            [
                [SK.None,     SK.None,     SK.None,     SK.None,     SK.None],
                [SK.None,     SK.None,     SK.None,     SK.None,     SK.None],
                [SK.Obstacle, SK.Obstacle, SK.Obstacle, SK.Obstacle, SK.Obstacle],
            ]
        };

    public static readonly Chunk CenterLaneBlockedWithObstacle =
        new()
        {
            Name = nameof(CenterLaneBlockedWithObstacle),
            Length = 5,
            Lanes = 3,
            Sprites =
            [
                [SK.None,     SK.None,     SK.None,     SK.None,     SK.None],
                [SK.Obstacle, SK.Obstacle, SK.Obstacle, SK.Obstacle, SK.Obstacle],
                [SK.None,     SK.None,     SK.None,     SK.None,     SK.None],
            ]
        };

    public static readonly Chunk LeftLaneBlockedWithObstacle =
        new()
        {
            Name = nameof(LeftLaneBlockedWithObstacle),
            Length = 5,
            Lanes = 3,
            Sprites =
            [
                [SK.Obstacle, SK.Obstacle, SK.Obstacle, SK.Obstacle, SK.Obstacle],
                [SK.None,     SK.None,     SK.None,     SK.None,     SK.None],
                [SK.None,     SK.None,     SK.None,     SK.None,     SK.None],
            ]
        };

    public static readonly Chunk LeftAndRightLaneBlockedWithObstacle =
        new()
        {
            Name = nameof(LeftAndRightLaneBlockedWithObstacle),
            Length = 5,
            Lanes = 3,
            Sprites =
            [
                [SK.Obstacle, SK.Obstacle, SK.Obstacle, SK.Obstacle, SK.Obstacle],
                [SK.None,     SK.None,     SK.None,     SK.None,     SK.None],
                [SK.Obstacle, SK.Obstacle, SK.Obstacle, SK.Obstacle, SK.Obstacle],
            ]
        };

    public static readonly Chunk PlayingCard3Variation1 =
        new()
        {
            Name = nameof(PlayingCard3Variation1),
            Length = 5,
            Lanes = 3,
            Sprites =
            [
                [SK.Obstacle, SK.None, SK.None,     SK.None, SK.None],
                [SK.None,     SK.None, SK.Obstacle, SK.None, SK.None],
                [SK.None,     SK.None, SK.None,     SK.None, SK.Obstacle],
            ]
        };

    public static readonly Chunk PlayingCard3Variation2 =
        new()
        {
            Name = nameof(PlayingCard3Variation2),
            Length = 5,
            Lanes = 3,
            Sprites =
            [
                [SK.None,     SK.None, SK.None,     SK.None, SK.Obstacle],
                [SK.None,     SK.None, SK.Obstacle, SK.None, SK.None],
                [SK.Obstacle, SK.None, SK.None,     SK.None, SK.None],
            ]
        };

    public static readonly Chunk ChicaneLeft =
        new()
        {
            Name = nameof(ChicaneLeft),
            Length = 5,
            Lanes = 3,
            Sprites =
            [
                [SK.Obstacle, SK.None, SK.None,     SK.None, SK.Obstacle],
                [SK.Obstacle, SK.None, SK.Obstacle, SK.None, SK.Obstacle],
                [SK.None,     SK.None, SK.Obstacle, SK.None, SK.None],
            ]
        };

    public static readonly Chunk ChicaneRight =
        new()
        {
            Name = nameof(ChicaneRight),
            Length = 5,
            Lanes = 3,
            Sprites =
            [
                [SK.None,     SK.None, SK.Obstacle, SK.None, SK.None],
                [SK.Obstacle, SK.None, SK.Obstacle, SK.None, SK.Obstacle],
                [SK.Obstacle, SK.None, SK.None,     SK.None, SK.Obstacle],
            ]
        };
}
