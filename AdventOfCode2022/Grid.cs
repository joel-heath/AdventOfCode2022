using System.Drawing;
using System.Text;

namespace AdventOfCode2022;
class Grid<T>
{
    private T[,] points;
    public int Width { get; }
    public int Height { get; }

    public Grid(int x, int y)
    {
        points = new T[x, y];
        Width = x;
        Height = y;
    }
    public Grid(int x, int y, T defaultValue) : this(x, y)
    {
        foreach (var p in AllPositions())
            this[p] = defaultValue;
    }

    public T this[Point c]
    {
        get { return points[c.X, c.Y]; }
        set { points[c.X, c.Y] = value; }
    }

    public bool IsInGrid(Point p) => p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;

    private readonly IEnumerable<Point> cardinalNeighbours = new Point[] { (0, -1), (1, 0), (0, 1), (-1, 0) };
    private readonly IEnumerable<Point> diagonalNeighbours = new Point[] { (1, -1), (1, 1), (-1, 1), (-1, -1) };
    public IEnumerable<Point> Neighbours(Point p, bool includeDiagonals = false)
    {
        var neighbours = includeDiagonals ? cardinalNeighbours.Concat(diagonalNeighbours) : cardinalNeighbours;
        return neighbours.Select(n => p + n).Where(IsInGrid);
    }

    public IEnumerable<Point> AllPositions()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                yield return (x, y);
            }
        }
    }

    public IEnumerable<Point> LineOut(Point start, int direction, bool inclusive)
    {
        if (!IsInGrid(start)) yield break;

        if (direction == 0) // North
        {
            for (int i = inclusive ? start.Y : start.Y - 1; i >= 0; i--)
            {
                yield return (start.X, i);
            }
        }
        else if (direction == 2) // South
        {
            for (int i = inclusive ? start.Y : start.Y + 1; i < Height; i++)
            {
                yield return (start.X, i);
            }
        }
        else if (direction == 3) // West
        {
            for (int i = inclusive ? start.X : start.X + 1; i >= 0; i--)
            {
                yield return (i, start.Y);
            }
        }
        else if (direction == 1) // East
        {
            for (int i = inclusive ? start.X : start.X + 1; i < Width; i++)
            {
                yield return (i, start.Y);
            }
        }
        else { throw new Exception("Invalid direction, may only be 0-3 (N,E,S,W)"); }
        
    }

    public IEnumerable<Point> LineTo(Point start, Point end, bool inclusive=true)
    {
        if (!IsInGrid(start) || !IsInGrid(end)) yield break;

        if (start.X == end.X)
        {
            int small = Math.Min(start.Y, end.Y);
            int large = Math.Max(start.Y, end.Y);

            for (int i = small; !inclusive && i < large || inclusive && i <= large; i++)
            {
                yield return (start.X, i);
            }
        }
        else if (start.Y == end.Y)
        {
            int small = Math.Min(start.X, end.X);
            int large = Math.Max(start.X, end.X);

            for (int i = small; !inclusive && i < large || inclusive && i <= large; i++)
            {
                yield return (start.X, i);
            }
        }
        else
        {
            throw new Exception($"Not a straight line between {start} and {end}");
        }
    }

    public override string ToString()
    {
        var s = new StringBuilder();
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                s.Append(points[x, y]!.ToString());
            }
            s.AppendLine();
        }
        return s.ToString();
    }
}

class Point
{
    private readonly int x;
    private readonly int y;
    public int X { get => x; }
    public int Y { get => y; }
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public static implicit operator Point((int X, int Y) coords) => new(coords.X, coords.Y);
    //public static implicit operator (int X, int Y)(Point p) => (p.X, p.Y);

    public void Deconstruct(out int x, out int y)
    {
        x = this.x;
        y = this.y;
    }

    public static Point operator +(Point a, Point b) => new(a.x + b.x, a.y + b.y);
    public static Point operator -(Point a, Point b) => new(a.x - b.x, a.y - b.y);
    public int this[int index] { get => index == 0 ? x : y; }

    //public bool Equals([AllowNull] Point p) => (this == null && p == null) || (p != null && x == p.x && y == p.y);
    public override bool Equals(object? obj) => obj is Point p && p.x.Equals(x) && p.y.Equals(y);
    public override int GetHashCode() => HashCode.Combine(x, y);

    public static bool operator ==(Point? a, Point? b) => (a is null && b is null) || (a is not null && b is not null && a.x == b.x && a.y == b.y);
    public static bool operator !=(Point? a, Point? b) => (a is not null || b is not null) && (a is null || b is null || a.x != b.x || a.y != b.y);

    public override string ToString() => $"({X}, {Y})";
}

class Coord
{
    private readonly int x, y, z;
    public int X { get => x; }
    public int Y { get => y; }
    public int Z { get => z; }

    public Coord(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static implicit operator Coord((int X, int Y, int Z) coords) => new(coords.X, coords.Y, coords.Z);

    public void Deconstruct(out int x, out int y, out int z)
    {
        x = this.x;
        y = this.y;
        z = this.z;
    }

    public int[] Coords() => new int[] { x, y, z };

    public Coord[] Adjacents() => new Coord[] { this + (0, 0, 1), this - (0, 0, 1), this + (0, 1, 0), this - (0, 1, 0), this + (1, 0, 0), this - (1, 0, 0) };
    //public Coord[] DiagonalAdjacents() => new Coord[] { this + (0, 0, 1), this - (0, 0, 1), this + (0, 1, 0), this - (0, 1, 0), this + (1, 0, 0), this - (1, 0, 0) };

    public static Coord operator +(Coord a, Coord b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
    public static Coord operator -(Coord a, Coord b) => new(a.x - b.x, a.y - b.y, a.z - b.z);
    public int this[int index] { get => index == 0 ? x : index == 1 ? y : z; }
    public override bool Equals(object? obj) => obj is Coord p && p.x.Equals(x) && p.y.Equals(y) && p.Z.Equals(z);
    public override int GetHashCode() => HashCode.Combine(x, y, z);

    public static bool operator ==(Coord a, Coord b) => (a is null && b is null) || (a is not null && b is not null && a.x == b.x && a.y == b.y && a.z == b.z);
    public static bool operator !=(Coord a, Coord b) => (a is not null || b is not null) && (a is null || b is null || a.x != b.x || a.y != b.y || a.z != b.z);
    public static bool operator <(Coord a, Coord b) => (a is not null && b is not null && a.x < b.x && a.y < b.y && a.z < b.z);
    public static bool operator >(Coord a, Coord b) => (a is not null && b is not null && a.x > b.x && a.y > b.y && a.z > b.z);
    public static bool operator <=(Coord a, Coord b) => (a is null && b is null) || (a is not null && b is not null && a.x <= b.x && a.y <= b.y && a.z <= b.z);
    public static bool operator >=(Coord a, Coord b) => (a is null && b is null) || (a is not null && b is not null && a.x >= b.x && a.y >= b.y && a.z >= b.z);
    public override string ToString() => $"({x}, {y}, {z})";
}