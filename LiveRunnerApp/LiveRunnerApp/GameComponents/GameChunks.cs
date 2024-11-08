using SK = LiveRunnerApp.GameComponents.SpriteKind;

namespace LiveRunnerApp.GameComponents;

public static class GameChunks
{
    public static readonly Chunk Empty =
        new()
        {
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
