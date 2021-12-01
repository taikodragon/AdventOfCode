using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2020
{

    class Day20 : ASolution
    {
        const int tileDims = 10;

        const int tileDataDim = tileDims - 2;
        const bool isOffset = true;

        //const int tileDataDim = tileDims;
        //const bool isOffset = false;

        enum Side
        {
            Top,Bottom,Left,Right, FlipTop, FlipBottom, FlipLeft, FlipRight
        }
        class Tile
        {
            readonly int tileDim = tileDims;
            public Tile() {
                ImageData = new bool[tileDims, tileDims];
            }
            public Tile(int altTileDim) {
                tileDim = altTileDim;
                ImageData = new bool[tileDim, tileDim];
            }
            public long TileId { get; set; }
            public bool[,] ImageData { get; set; }
            public IntCoord Origin { get; set; }
            public Dictionary<Side, string> Sides { get; set; } = new Dictionary<Side, string>();

            public void FlipVert() {
                ImageData = ImageData.FlipVertically();
                RebuildSides();
            }
            public void FlipHoriz() {
                ImageData = ImageData.FlipHorizontally();
                RebuildSides();
            }
            public void RotateCounterClockwise() {
                ImageData = ImageData.RotateCounterClockwise();
                RebuildSides();
            }
            public void RotateClockwise() {
                ImageData = ImageData.RotateClockwise();
                RebuildSides();
            }
            public void RebuildSides() {
                var sides = new Dictionary<Side, bool[]>() {
                    { Side.Top, new bool[tileDim] },
                    { Side.Bottom, new bool[tileDim] },
                    { Side.Left, new bool[tileDim] },
                    { Side.Right, new bool[tileDim] },
                };
                for( int x = 0; x < tileDim; x++ ) {
                    for( int y = 0; y < tileDim; y++ ) {
                        if( ImageData[x, y] ) {
                            if( x == 0 ) sides[Side.Top][y] = true;
                            if( x == tileDim - 1 ) sides[Side.Bottom][y] = true;
                            if( y == 0 ) sides[Side.Left][x] = true;
                            if( y == tileDim - 1 ) sides[Side.Right][x] = true;
                        }
                    }
                }

                sides[Side.FlipTop] = sides[Side.Top].Reverse().ToArray();
                sides[Side.FlipBottom] = sides[Side.Bottom].Reverse().ToArray();
                sides[Side.FlipLeft] = sides[Side.Left].Reverse().ToArray();
                sides[Side.FlipRight] = sides[Side.Right].Reverse().ToArray();

                Sides.Clear();
                foreach(var pair in sides) {
                    Sides.Add(pair.Key, string.Join("", pair.Value.Select(b => b ? 1 : 0)));
                }
            }
            public void Print(int spaceDim, int dims = 0, List<long> tiles = null) {
                StringBuilder sb = new StringBuilder(ImageData.Length);
                for(int x = 0; x < tileDim; x++) {
                    if( x % spaceDim == 0 ) sb.AppendLine();
                    if( dims != 0 && x % spaceDim == 0 ) {
                        int padAmt = (spaceDim - 4) / 2;
                        sb.AppendLine(string.Join(" ",tiles.GetRange((x / spaceDim)*dims, dims).Select(l => l.ToString().PadLeft(spaceDim - padAmt).PadRight(spaceDim))));
                    }
                    for( int y = 0; y < tileDim; y++) {
                        if( y > 0 && y % spaceDim == 0 ) sb.Append(' ');
                        sb.Append(ImageData[x, y] ? '#' : '.');
                    }
                    sb.AppendLine();
                }
                Trace.Write(sb);
            }
        }


            Dictionary<long, Tile> tiles = new Dictionary<long, Tile>();
        Dictionary<string, List<(long id, Side side)>> sideCache = new Dictionary<string, List<(long, Side)>>();
        int dim;
        public Day20() : base(20, 2020, "Jurassic Jigsaw", false)
        {
            

            string[] rawTiles = Input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

            foreach(string rawTile in rawTiles) {
                var tileLines = rawTile.SplitByNewline();
                long tileId = long.Parse(tileLines[0].Replace("Tile ", string.Empty).Trim(':'));
                Tile img = new Tile() { TileId = tileId };
                tileLines.Skip(1)
                    .Select((l, i) => {
                        l.Select((c, j) => {
                            if( c == '#' ) {
                                img.ImageData[i, j] = true;
                            }
                            return false;
                        }).ToList();
                       return false;
                    }).ToList();

                img.RebuildSides();

                tiles.Add(img.TileId, img);
            }

            BuildSideCache();
            dim = (int)Math.Round(Math.Sqrt(tiles.Count));

        }

        protected override string SolvePartOne()
        {
            Dictionary<long, int> shortList = new Dictionary<long, int>();
            // try to assemble the image
            foreach(var sidePair in sideCache) {
                long firstTileId = sidePair.Value[0].Item1;
                if( sidePair.Value.Count == 1 ) {
                    if(shortList.ContainsKey(firstTileId) ) {
                        shortList[firstTileId]++;
                    } else {
                        shortList[firstTileId] = 1;
                    }
                }
            }

            return shortList.Where(s => s.Value == 4).Select(s => s.Key).Aggregate(  (lhs,rhs) => lhs*rhs ).ToString();
        }

        const int seaMonsterYDim = 19, seaMonsterXDim = 2;
        List<IntCoord> seaMonster = new List<IntCoord> {
            new IntCoord(0, 18),
            new IntCoord(1,  0),new IntCoord(1, 5),new IntCoord(1, 6),new IntCoord(1, 11), new IntCoord(1, 12), new IntCoord(1, 17), new IntCoord(1, 18), new IntCoord(1, 19),
            new IntCoord(2,  1),new IntCoord(2, 4),new IntCoord(2, 7),new IntCoord(2, 10), new IntCoord(2, 13), new IntCoord(2, 16)
        };

        protected override string SolvePartTwo()
        {
            //foreach( var side in tiles.First().Value.Sides ) {
            //    Trace.WriteLine($"{side.Key}: {side.Value.Replace('1', '#').Replace('0', '.')}");
            //}
            //return "FAIL";

            List<Side> flipSides = new List<Side> { Side.FlipTop, Side.FlipBottom, Side.FlipLeft, Side.FlipRight };

            Dictionary<int, Dictionary<int, bool>> builtImage = new Dictionary<int, Dictionary<int, bool>>();

            List<long> locked = new List<long>();
            Queue<long> toBePlacedNext = new Queue<long>();
            toBePlacedNext.Enqueue(tiles.First().Value.TileId);

            while(toBePlacedNext.Count > 0 ) {
                long tileId = toBePlacedNext.Dequeue();
                if( locked.Contains(tileId) ) continue;

                Tile tile = tiles[tileId];
                if( tile.Origin == null ) tile.Origin = new IntCoord(0, 0);
                locked.Add(tileId);

                CopyTo(tile,
                    tile.Origin.X, tile.Origin.X + tileDataDim, 1,
                    tile.Origin.Y, tile.Origin.Y + tileDataDim, 1,
                    builtImage);

                Tile top = OtherSide(tile, Side.Top);
                if( top != null && !locked.Contains(top.TileId)) {
                    if( top.Origin == null )
                        top.Origin = new IntCoord(tile.Origin.X - tileDataDim, tile.Origin.Y);
                    toBePlacedNext.Enqueue(top.TileId);
                }
                Tile bottom = OtherSide(tile, Side.Bottom);
                if( bottom != null && !locked.Contains(bottom.TileId) ) {
                    if( bottom.Origin == null )
                        bottom.Origin = new IntCoord(tile.Origin.X + tileDataDim, tile.Origin.Y);
                    toBePlacedNext.Enqueue(bottom.TileId);
                }
                Tile left = OtherSide(tile, Side.Left);
                if( left != null && !locked.Contains(left.TileId) ) {
                    if( left.Origin == null )
                        left.Origin = new IntCoord(tile.Origin.X, tile.Origin.Y - tileDataDim);
                    toBePlacedNext.Enqueue(left.TileId);
                }
                Tile right = OtherSide(tile, Side.Right);
                if( right != null && !locked.Contains(right.TileId) ) {
                    if( right.Origin == null )
                        right.Origin = new IntCoord(tile.Origin.X, tile.Origin.Y + tileDataDim);
                    toBePlacedNext.Enqueue(right.TileId);
                }
            }

            bool[,] image = new bool[dim * tileDataDim, dim * tileDataDim];
            int i = 0, j = 0;
            foreach(int x in builtImage.Keys.OrderBy(k => k)) {
                j = 0;
                foreach(int y in builtImage[x].Keys.OrderBy(k => k)) {
                    image[i, j] = builtImage[x][y];
                    j++;
                }
                i++;
            }

            PrintTile(image);

            List<bool[,]> variants = new List<bool[,]>();


            variants.Add(image);
            Trace.WriteLine("BUILT"); PrintTile(variants[variants.Count - 1]);

            // ClockWise
            variants.Add(image.RotateClockwise());
            Trace.WriteLine("CW"); PrintTile(variants[variants.Count - 1]);

            // Counter ClocKWise
            variants.Add(image.RotateCounterClockwise());
            Trace.WriteLine("CCW"); PrintTile(variants[variants.Count - 1]);

            variants.Add(image.FlipVertically());
            Trace.WriteLine("FLIP V"); PrintTile(variants[variants.Count - 1]);

            var hFlip = image.FlipHorizontally();
            variants.Add(hFlip);
            Trace.WriteLine("FLIP H"); PrintTile(variants[variants.Count - 1]);

            // ClockWise
            variants.Add(hFlip.RotateClockwise());
            Trace.WriteLine("FLIP H CW"); PrintTile(variants[variants.Count - 1]);

            // Counter ClocKWise
            variants.Add(hFlip.RotateCounterClockwise());
            Trace.WriteLine("FLIP H CCW"); PrintTile(variants[variants.Count - 1]);

            variants.Add(hFlip.FlipVertically());
            Trace.WriteLine("FLIP H FLIP V"); PrintTile(variants[variants.Count - 1]);

            int imgStride = dim * (tileDataDim);
            foreach(var rawImg in variants) {
                List<IntCoord> exempt = new List<IntCoord>();
                int foundMonsters = 0;

                for(int x = 0; x < imgStride - seaMonsterXDim; x++ ) {
                    for( int y = 0; y < imgStride - seaMonsterYDim; y++ ) {
                        if( seaMonster.TrueForAll(dydx => rawImg[x + dydx.X, y + dydx.Y]) ) {
                            foundMonsters++;
                            seaMonster.ForEach(dydx => exempt.Add(new IntCoord(x + dydx.X, y + dydx.Y)));
                        }
                    }
                }
                if(foundMonsters > 0) {
                    Trace.WriteLine($"FOUND {foundMonsters} MONSTERS!");
                    int roughness = 0;
                    for( int x = 0; x < imgStride; x++ ) {
                        for( int y = 0; y < imgStride; y++ ) {
                            if( rawImg[x, y] && !exempt.Contains(new IntCoord(x, y)) ) roughness++;
                        }
                    }
                    Trace.WriteLine($"This sea was {roughness} rough!");
                }
            }
            return "FAIL";
        }

        void CopyTo(Tile tile, int xMin, int xMax, int xInc, int yMin, int yMax, int yInc, Dictionary<int,Dictionary<int,bool>> image) {
            for(int x = xMin; xInc > 0 ? x < xMax : x > xMax; x += xInc ) {
                if( !image.ContainsKey(x) ) { image[x] = new Dictionary<int, bool>(dim * (tileDataDim)); }
                for(int y = yMin; yInc > 0 ? y < yMax : y > yMax; y += yInc ) {
                    image[x][y] = tile.ImageData[Math.Abs(x - xMin) + (isOffset?1:0), Math.Abs(y - yMin) + (isOffset ? 1 : 0)];
                }
            }
        }

        void BuildSideCache() {
            sideCache.Clear();
            foreach( var img in tiles.Values) {
                foreach( var pair in img.Sides ) {
                    if( sideCache.TryGetValue(pair.Value, out var sharingSides) ) {
                        sharingSides.Add((img.TileId, pair.Key));
                    }
                    else {
                        sideCache[pair.Value] = new List<(long, Side)> { (img.TileId, pair.Key) };
                    }
                }
            }
        }

        Tile OtherSide(Tile tile, Side archorSide) {
            var sides = sideCache[tile.Sides[archorSide]];
            if( sides.Count == 1 ) return null;
            
            Tile other = null;
            Side otherSide;
            if( sides[0].id == tile.TileId ) {
                other = tiles[sides[1].id];
                otherSide = sides[1].side;
            } else {
                other = tiles[sides[0].id];
                otherSide = sides[0].side;
            }

            if( Opposite(archorSide) != otherSide ) {
                FlipRotate(archorSide, otherSide, other, tile.Sides[archorSide]);
                BuildSideCache();
            }
            return other;
        }

        void FlipRotate(Side archorSide, Side otherSide, Tile tile, string archorSideKey) {
            switch(archorSide) {
                default: throw new ArgumentException("archorSide unknown");
                case Side.Top:
                    switch( otherSide ) {
                        case Side.Bottom: // matching
                            break;
                        case Side.Top:
                            tile.FlipVert();
                            break;
                        case Side.Left:
                            tile.RotateClockwise();
                            break;
                        case Side.Right:
                            tile.RotateClockwise();
                            tile.FlipVert();
                            break;
                        case Side.FlipTop:
                            tile.FlipVert();
                            tile.FlipHoriz();
                            break;
                        case Side.FlipBottom:
                            tile.FlipVert();
                            break;
                        case Side.FlipLeft:
                            tile.FlipHoriz();
                            tile.RotateCounterClockwise();
                            break;
                        case Side.FlipRight:
                            tile.RotateCounterClockwise();
                            break;
                    }
                    break;
                case Side.Bottom:
                    switch( otherSide ) {
                        case Side.Top: // matching
                            break;
                        case Side.Bottom:
                            tile.FlipHoriz();
                            break;
                        case Side.Left:
                            tile.RotateClockwise();
                            tile.FlipVert();
                            break;
                        case Side.Right:
                            tile.RotateClockwise();
                            break;
                        case Side.FlipTop:
                            tile.FlipVert();
                            break;
                        case Side.FlipBottom:
                            tile.FlipVert();
                            tile.FlipHoriz();
                            break;
                        case Side.FlipLeft:
                            tile.FlipHoriz();
                            tile.RotateClockwise();
                            break;
                        case Side.FlipRight:
                            tile.FlipHoriz();
                            tile.RotateCounterClockwise();
                            break;
                    }
                    break;
                case Side.Left:
                    switch( otherSide ) {
                        case Side.Right: // matching
                            break;
                        case Side.Top:
                            tile.RotateCounterClockwise();
                            break;
                        case Side.Bottom:
                            tile.RotateCounterClockwise();
                            tile.FlipHoriz();
                            break;
                        case Side.Left:
                            tile.FlipVert();
                            break;
                        case Side.FlipTop:
                            tile.FlipVert();
                            tile.RotateClockwise();
                            break;
                        case Side.FlipBottom:
                            tile.RotateClockwise();
                            break;
                        case Side.FlipLeft:
                            tile.FlipHoriz();
                            tile.FlipVert();
                            break;
                        case Side.FlipRight:
                            tile.FlipVert();
                            break;
                    }
                    break;
                case Side.Right:
                    switch( otherSide ) {
                        case Side.Left: // matching
                            break;
                        case Side.Top:
                            tile.RotateClockwise();
                            tile.FlipVert();
                            break;
                        case Side.Bottom:
                            tile.RotateCounterClockwise();
                            break;
                        case Side.Right:
                            tile.FlipVert();
                            break;
                        case Side.FlipTop:
                            tile.FlipHoriz();
                            tile.RotateCounterClockwise();
                            tile.FlipHoriz();
                            break;
                        case Side.FlipBottom:
                            tile.FlipVert();
                            tile.RotateClockwise();
                            break;
                        case Side.FlipLeft:
                            tile.FlipVert();
                            break;
                        case Side.FlipRight:
                            tile.FlipHoriz();
                            tile.FlipVert();
                            break;
                    }
                    break;
            }
            if( archorSideKey != tile.Sides[Opposite(archorSide)] ) {
                Debugger.Break();
                throw new Exception("FAIL");
            }
        }

        static Side Opposite(Side side) {
            switch( side ) {
                case Side.Top:
                    return Side.Bottom;
                case Side.Bottom:
                    return Side.Top;
                case Side.Left:
                    return Side.Right;
                case Side.Right:
                    return Side.Left;
                case Side.FlipTop:
                    return Side.FlipBottom;
                case Side.FlipBottom:
                    return Side.FlipTop;
                case Side.FlipLeft:
                    return Side.FlipRight;
                case Side.FlipRight:
                    return Side.FlipLeft;
                default:
                    throw new ArgumentException("Unknown enum value");
            }
        }

        static void PrintBlocks(Tile[,] blk) {
            int xLength = blk.GetLength(0),
                yLength = blk.GetLength(1);
            StringBuilder sb = new StringBuilder();
            for( int x = 0; x < xLength; x++ ) {
                for( int y = 0; y < yLength; y++ ) {
                    sb.Append(blk[x, y].TileId);
                    sb.Append(" ");
                }
                sb.AppendLine();
            }
            Trace.Write(sb);
        }

        void PrintTile(bool[,] arr ) {
            new Tile(arr.GetLength(0)) { ImageData = arr }.Print(tileDataDim);
        }
    }
}
