using UnityEngine;

namespace Chess.Game
{
    [CreateAssetMenu(menuName = "Theme/Pieces")]
    public class PieceTheme : ScriptableObject
    {
        public PieceSprites whitePieces;
        public PieceSprites blackPieces;

        public Sprite Dut(int piece)
        {
            PieceSprites pieceSprites = Piece.IsColour(piece, Piece.White) ? whitePieces : blackPieces;
            switch (Piece.PieceType(piece))
            {
                case Piece.Pawn:
                    return pieceSprites.pawn;
                case Piece.Rook:
                    return pieceSprites.rook;
                case Piece.Knight:
                    return pieceSprites.knight;
                case Piece.Bishop:
                    return pieceSprites.bishop;
                case Piece.Queen:
                    return pieceSprites.queen;
                case Piece.King:
                    return pieceSprites.king;
                default:
                    if (piece != 0)
                    {
                        Debug.Log(piece);
                    }

                    return null;
            }
        }

        public Sprite GetPieceSprite(int piece, Coord coord)
        {
            // Debug.LogError($"Get Piece Sprite: {coord.fileIndex} - {coord.rankIndex}");
            PieceSprites pieceSprites = Piece.IsColour(piece, Piece.White) ? whitePieces : blackPieces;
            // bool isFirstMove = pieceIndex != -1 && BlindChessController.instance.PieceSelected[pieceIndex];

            // var dutBlind = new DutBlind(coord.fileIndex, coord.rankIndex);
            if (!BlindChessController.instance.isBlindChess)
            {
                switch (Piece.PieceType(piece))
                {
                    case Piece.Pawn:
                        return pieceSprites.pawn;
                    case Piece.Rook:
                        return pieceSprites.rook;
                    case Piece.Knight:
                        return pieceSprites.knight;
                    case Piece.Bishop:
                        return pieceSprites.bishop;
                    case Piece.Queen:
                        return pieceSprites.queen;
                    case Piece.King:
                        return pieceSprites.king;
                }
            }
            else
            {
                if (BlindChessController.instance.MatchDutBlind(coord.fileIndex, coord.rankIndex))
                {
                    // Debug.Log($"Contains: {dutBlind.file} - {dutBlind.rank}");
                    switch (Piece.PieceType(piece))
                    {
                        case Piece.Pawn:
                            return pieceSprites.pawn;
                        case Piece.Rook:
                            return pieceSprites.rook;
                        case Piece.Knight:
                            return pieceSprites.knight;
                        case Piece.Bishop:
                            return pieceSprites.bishop;
                        case Piece.Queen:
                            return pieceSprites.queen;
                        case Piece.King:
                            return pieceSprites.king;
                    }
                }
                else
                {
                    switch (Piece.PieceType(piece))
                    {
                        case Piece.Pawn:
                            return pieceSprites.blind;
                        case Piece.Rook:
                            return pieceSprites.blind;
                        case Piece.Knight:
                            return pieceSprites.blind;
                        case Piece.Bishop:
                            return pieceSprites.blind;
                        case Piece.Queen:
                            return pieceSprites.blind;
                        case Piece.King:
                            return pieceSprites.king;
                    }
                }
            }
            return null;
        }

        [System.Serializable]
        public class PieceSprites
        {
            public Sprite pawn, rook, knight, bishop, queen, king, blind;

            public Sprite this[int i]
            {
                get { return new Sprite[] { pawn, rook, knight, bishop, queen, king, blind }[i]; }
            }
        }
    }
}