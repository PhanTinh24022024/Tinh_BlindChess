﻿using System;
using UnityEngine;

namespace Chess
{
    using System.Collections.Generic;
    using Chess.Game;

    public class Board
    {
        public const int WhiteIndex = 0;
        public const int BlackIndex = 1;

        // Stores piece code for each square on the board.
        // Piece code is defined as piecetype | colour code
        public int[] Square;

        public bool WhiteToMove;
        public int ColourToMove;
        public int OpponentColour;
        public int ColourToMoveIndex;
        SpriteRenderer[,] squarePieceRenderers;

        // Bits 0-3 store white and black kingside/queenside castling legality
        // Bits 4-7 store file of ep square (starting at 1, so 0 = no ep square)
        // Bits 8-13 captured piece
        // Bits 14-... fifty mover counter
        Stack<uint> gameStateHistory;
        public uint currentGameState;

        public int plyCount; // Total plies played in game
        public int fiftyMoveCounter; // Num ply since last pawn move or capture

        public ulong ZobristKey;

        /// List of zobrist keys 
        public Stack<ulong> RepetitionPositionHistory;

        public int[] KingSquare; // index of square of white and black king

        public PieceList[] rooks;
        public PieceList[] bishops;
        public PieceList[] queens;
        public PieceList[] knights;
        public PieceList[] pawns;

        PieceList[] allPieceLists;

        const uint whiteCastleKingsideMask = 0b1111111111111110;
        const uint whiteCastleQueensideMask = 0b1111111111111101;
        const uint blackCastleKingsideMask = 0b1111111111111011;
        const uint blackCastleQueensideMask = 0b1111111111110111;

        const uint whiteCastleMask = whiteCastleKingsideMask & whiteCastleQueensideMask;
        const uint blackCastleMask = blackCastleKingsideMask & blackCastleQueensideMask;

        PieceList GetPieceList(int pieceType, int colourIndex)
        {
            return allPieceLists[colourIndex * 8 + pieceType];
        }

        // Make a move on the board
        // The inSearch parameter controls whether this move should be recorded in the game history (for detecting three-fold repetition)
        private Dictionary<int, string> pieceIdentifiers = new Dictionary<int, string>();
        private HashSet<string> movedPieces = new HashSet<string>();
        private int whitePawnCounter = 1, blackPawnCounter = 1;
        private int whiteRookCounter = 1, blackRookCounter = 1;
        private int whiteKnightCounter = 1, blackKnightCounter = 1;
        private int whiteBishopCounter = 1, blackBishopCounter = 1;
        private int whiteQueenCounter = 1, blackQueenCounter = 1;
        private int whiteKingCounter = 1, blackKingCounter = 1;

        private string GetPieceIdentifier(int pieceType, bool isWhite)
        {
            string color = isWhite ? "White" : "Black";
            switch (pieceType)
            {
                case Piece.Pawn:
                    return $"{color} Pawn{(isWhite ? whitePawnCounter++ : blackPawnCounter++)}";
                case Piece.Rook:
                    return $"{color} Rook{(isWhite ? whiteRookCounter++ : blackRookCounter++)}";
                case Piece.Knight:
                    return $"{color} Knight{(isWhite ? whiteKnightCounter++ : blackKnightCounter++)}";
                case Piece.Bishop:
                    return $"{color} Bishop{(isWhite ? whiteBishopCounter++ : blackBishopCounter++)}";
                case Piece.Queen:
                    return $"{color} Queen{(isWhite ? whiteQueenCounter++ : blackQueenCounter++)}";
                case Piece.King:
                    return $"{color} King{(isWhite ? whiteKingCounter++ : blackKingCounter++)}";
                default:
                    return "Unknown";
            }
        }

        public void MakeMove(Move move, bool inSearch = false)
        {
            MovePiece(move, inSearch);
        }

        private void MovePiece(Move move, bool inSearch = false)
        {
            uint oldEnPassantFile = (currentGameState >> 4) & 15;
            uint originalCastleState = currentGameState & 15;
            uint newCastleState = originalCastleState;
            currentGameState = 0;

            int opponentColourIndex = 1 - ColourToMoveIndex;
            int moveFrom = move.StartSquare;
            int moveTo = move.TargetSquare;
            int capturedPieceType = Piece.PieceType(Square[moveTo]);
            int movePiece = Square[moveFrom];
            int movePieceType = Piece.PieceType(movePiece);

            int moveFlag = move.MoveFlag;
            bool isPromotion = move.IsPromotion;
            bool isEnPassant = moveFlag == Move.Flag.EnPassantCapture;

            string identifier;
            if (!pieceIdentifiers.TryGetValue(moveFrom, out identifier))
            {
                identifier = GetPieceIdentifier(movePieceType, WhiteToMove);
                pieceIdentifiers[moveFrom] = identifier;
            }
            if (!inSearch)
            {
                if (!movedPieces.Contains(identifier))
                {
                    Debug.LogError($"{identifier} FirstMove");
                    movedPieces.Add(identifier); //TODO: Đánh dấu quân cờ này đã di chuyển
                    switch (movePieceType)
                    {
                        case Piece.Rook:
                            int rookIndex = identifier.EndsWith("Rook1") ? 0 : 1;
                            if (WhiteToMove) BlindChessController.instance.isWhiteRookFirstMove[rookIndex] = false;
                            else BlindChessController.instance.isBlackRookFirstMove[rookIndex] = false;
                            break;

                        case Piece.Pawn:
                            int pawnIndex = identifier.EndsWith("Pawn1") ? 0 :
                                identifier.EndsWith("Pawn2") ? 1 :
                                identifier.EndsWith("Pawn3") ? 2 :
                                identifier.EndsWith("Pawn4") ? 3 :
                                identifier.EndsWith("Pawn5") ? 4 :
                                identifier.EndsWith("Pawn6") ? 5 :
                                identifier.EndsWith("Pawn7") ? 6 : 7;
                            if (WhiteToMove)
                                BlindChessController.instance.isWhitePawnFirstMove[pawnIndex] = false;
                            else BlindChessController.instance.isBlackPawnFirstMove[pawnIndex] = false;
                            break;

                        case Piece.Knight:
                            int knightIndex = identifier.EndsWith("Knight1") ? 0 : 1;
                            if (WhiteToMove) BlindChessController.instance.isWhiteKnightFirstMove[knightIndex] = false;
                            else BlindChessController.instance.isBlackKnightFirstMove[knightIndex] = false;
                            break;

                        case Piece.Bishop:
                            int bishopIndex = identifier.EndsWith("Bishop1") ? 0 : 1;
                            if (WhiteToMove) BlindChessController.instance.isWhiteBishopFirstMove[bishopIndex] = false;
                            else BlindChessController.instance.isBlackBishopFirstMove[bishopIndex] = false;
                            break;

                        case Piece.Queen:
                            if (WhiteToMove) BlindChessController.instance.isWhiteQueenFirstMove = false;
                            else BlindChessController.instance.isBlackQueenFirstMove = false;
                            break;
                    }
                }

                pieceIdentifiers.Remove(moveFrom);
                pieceIdentifiers[moveTo] = identifier;
            }


            // Handle captures
            currentGameState |= (ushort)(capturedPieceType << 8);
            if (capturedPieceType != 0 && !isEnPassant)
            {
                ZobristKey ^= Zobrist.piecesArray[capturedPieceType, opponentColourIndex, moveTo];
                GetPieceList(capturedPieceType, opponentColourIndex).RemovePieceAtSquare(moveTo);
                if (!inSearch)
                {
                    if (WhiteToMove)
                    {
                        Debug.LogError(
                            $"WhiteTurn: capturedPieceType: {capturedPieceType}/ MoveTo: {moveTo}/ PieceType: {movePieceType}");
                    }
                    else
                    {
                        Debug.LogError(
                            $"BlackTurn: capturedPieceType: {capturedPieceType}/ MoveTo: {moveTo}/ PieceType: {movePieceType}");
                    }

                    //TODO:Xu ly an quan co: capturedPieceType(loai quan bi an)/ moveTo(vi tri an)/ movePieceType(loai quan an)
                    BlindChessController.instance.UpdateNumberOfCapturedPieces(WhiteToMove, capturedPieceType,
                        BlindChessController.instance.humanPlayWhite);
                    //1 vua
                    //2 tot
                    //3 ma
                    //5 tuong
                    //6 xe
                    //7 hau
                }
            }

            // Move pieces in piece lists
            if (movePieceType == Piece.King)  
            {
                KingSquare[ColourToMoveIndex] = moveTo;
                newCastleState &= (WhiteToMove) ? whiteCastleMask : blackCastleMask;
            }
            else
            {
                GetPieceList(movePieceType, ColourToMoveIndex).MovePiece(moveFrom, moveTo);
            }

            int pieceOnTargetSquare = movePiece;


            // Handle promotion
            if (isPromotion) //TODO: xu ly phong hau
            {
                int promoteType = 0;
                switch (moveFlag)
                {
                    case Move.Flag.PromoteToQueen:
                        promoteType = Piece.Queen;
                        queens[ColourToMoveIndex].AddPieceAtSquare(moveTo);
                        break;
                    case Move.Flag.PromoteToRook:
                        promoteType = Piece.Rook;
                        rooks[ColourToMoveIndex].AddPieceAtSquare(moveTo);
                        break;
                    case Move.Flag.PromoteToBishop:
                        promoteType = Piece.Bishop;
                        bishops[ColourToMoveIndex].AddPieceAtSquare(moveTo);
                        break;
                    case Move.Flag.PromoteToKnight:
                        promoteType = Piece.Knight;
                        knights[ColourToMoveIndex].AddPieceAtSquare(moveTo);
                        break;
                }

                pieceOnTargetSquare = promoteType | ColourToMove;
                pawns[ColourToMoveIndex].RemovePieceAtSquare(moveTo);
                if (!inSearch)
                {
                    Debug.LogError($"phong hau");
                }
            }
            else
            {
                // Handle other special moves (en-passant, and castling)
                switch (moveFlag)
                {
                    case Move.Flag.EnPassantCapture:
                        int epPawnSquare = moveTo + ((ColourToMove == Piece.White) ? -8 : 8);
                        currentGameState |= (ushort)(Square[epPawnSquare] << 8); // add pawn as capture type
                        Square[epPawnSquare] = 0; // clear ep capture square
                        pawns[opponentColourIndex].RemovePieceAtSquare(epPawnSquare);
                        ZobristKey ^= Zobrist.piecesArray[Piece.Pawn, opponentColourIndex, epPawnSquare];
                        break;
                    case Move.Flag.Castling:
                        bool kingside = moveTo == BoardRepresentation.g1 || moveTo == BoardRepresentation.g8;
                        int castlingRookFromIndex = (kingside) ? moveTo + 1 : moveTo - 2;
                        int castlingRookToIndex = (kingside) ? moveTo - 1 : moveTo + 1;

                        Square[castlingRookFromIndex] = Piece.None;
                        Square[castlingRookToIndex] = Piece.Rook | ColourToMove;

                        rooks[ColourToMoveIndex].MovePiece(castlingRookFromIndex, castlingRookToIndex);
                        ZobristKey ^= Zobrist.piecesArray[Piece.Rook, ColourToMoveIndex, castlingRookFromIndex];
                        ZobristKey ^= Zobrist.piecesArray[Piece.Rook, ColourToMoveIndex, castlingRookToIndex];
                        break;
                }
            }

            // Update the board representation:
            Square[moveTo] = pieceOnTargetSquare;
            Square[moveFrom] = 0;

            // Pawn has moved two forwards, mark file with en-passant flag
            if (moveFlag == Move.Flag.PawnTwoForward)
            {
                int file = BoardRepresentation.FileIndex(moveFrom) + 1;
                currentGameState |= (ushort)(file << 4);
                ZobristKey ^= Zobrist.enPassantFile[file];
            }

            // Piece moving to/from rook square removes castling right for that side
            if (originalCastleState != 0)
            {
                if (moveTo == BoardRepresentation.h1 || moveFrom == BoardRepresentation.h1)
                {
                    newCastleState &= whiteCastleKingsideMask;
                }
                else if (moveTo == BoardRepresentation.a1 || moveFrom == BoardRepresentation.a1)
                {
                    newCastleState &= whiteCastleQueensideMask;
                }

                if (moveTo == BoardRepresentation.h8 || moveFrom == BoardRepresentation.h8)
                {
                    newCastleState &= blackCastleKingsideMask;
                }
                else if (moveTo == BoardRepresentation.a8 || moveFrom == BoardRepresentation.a8)
                {
                    newCastleState &= blackCastleQueensideMask;
                }
            }

            // Update zobrist key with new piece position and side to move
            ZobristKey ^= Zobrist.sideToMove;
            ZobristKey ^= Zobrist.piecesArray[movePieceType, ColourToMoveIndex, moveFrom];
            ZobristKey ^= Zobrist.piecesArray[Piece.PieceType(pieceOnTargetSquare), ColourToMoveIndex, moveTo];

            if (oldEnPassantFile != 0)
                ZobristKey ^= Zobrist.enPassantFile[oldEnPassantFile];

            if (newCastleState != originalCastleState)
            {
                ZobristKey ^= Zobrist.castlingRights[originalCastleState]; // remove old castling rights state
                ZobristKey ^= Zobrist.castlingRights[newCastleState]; // add new castling rights state
            }

            currentGameState |= newCastleState;
            currentGameState |= (uint)fiftyMoveCounter << 14;
            gameStateHistory.Push(currentGameState);

            // Change side to move
            WhiteToMove = !WhiteToMove;
            ColourToMove = (WhiteToMove) ? Piece.White : Piece.Black;
            OpponentColour = (WhiteToMove) ? Piece.Black : Piece.White;
            ColourToMoveIndex = 1 - ColourToMoveIndex;
            plyCount++;
            fiftyMoveCounter++;
            //TODO: Move Piece Type
            if (!inSearch)
            {
                if (movePieceType == Piece.Pawn || capturedPieceType != Piece.None)
                {
                    RepetitionPositionHistory.Clear();
                    fiftyMoveCounter = 0;
                }
                else
                {
                    RepetitionPositionHistory.Push(ZobristKey);
                }
            }
            //1 vua
            //2 tot
            //3 ma
            //5 tuong
            //6 xe
            //7 hau
        }

        // Undo a move previously made on the board
        public void UnmakeMove(Move move, bool inSearch = false)
        {
            //int opponentColour = ColourToMove;
            int opponentColourIndex = ColourToMoveIndex;
            bool undoingWhiteMove = OpponentColour == Piece.White;
            ColourToMove = OpponentColour; // side who made the move we are undoing
            OpponentColour = (undoingWhiteMove) ? Piece.Black : Piece.White;
            ColourToMoveIndex = 1 - ColourToMoveIndex;
            WhiteToMove = !WhiteToMove;

            uint originalCastleState = currentGameState & 0b1111;

            int capturedPieceType = ((int)currentGameState >> 8) & 63;
            int capturedPiece = (capturedPieceType == 0) ? 0 : capturedPieceType | OpponentColour;

            int movedFrom = move.StartSquare;
            int movedTo = move.TargetSquare;
            int moveFlags = move.MoveFlag;
            bool isEnPassant = moveFlags == Move.Flag.EnPassantCapture;
            bool isPromotion = move.IsPromotion;

            int toSquarePieceType = Piece.PieceType(Square[movedTo]);
            int movedPieceType = (isPromotion) ? Piece.Pawn : toSquarePieceType;

            // Update zobrist key with new piece position and side to move
            ZobristKey ^= Zobrist.sideToMove;
            ZobristKey ^=
                Zobrist.piecesArray
                    [movedPieceType, ColourToMoveIndex, movedFrom]; // add piece back to square it moved from
            ZobristKey ^=
                Zobrist.piecesArray
                    [toSquarePieceType, ColourToMoveIndex, movedTo]; // remove piece from square it moved to

            uint oldEnPassantFile = (currentGameState >> 4) & 15;
            if (oldEnPassantFile != 0)
                ZobristKey ^= Zobrist.enPassantFile[oldEnPassantFile];

            // ignore ep captures, handled later
            if (capturedPieceType != 0 && !isEnPassant)
            {
                ZobristKey ^= Zobrist.piecesArray[capturedPieceType, opponentColourIndex, movedTo];
                GetPieceList(capturedPieceType, opponentColourIndex).AddPieceAtSquare(movedTo);
            }

            // Update king index
            if (movedPieceType == Piece.King)
            {
                KingSquare[ColourToMoveIndex] = movedFrom;
            }
            else if (!isPromotion)
            {
                GetPieceList(movedPieceType, ColourToMoveIndex).MovePiece(movedTo, movedFrom);
            }

            // put back moved piece
            Square[movedFrom] =
                movedPieceType |
                ColourToMove; // note that if move was a pawn promotion, this will put the promoted piece back instead of the pawn. Handled in special move switch
            Square[movedTo] = capturedPiece; // will be 0 if no piece was captured

            if (isPromotion)
            {
                pawns[ColourToMoveIndex].AddPieceAtSquare(movedFrom);
                switch (moveFlags)
                {
                    case Move.Flag.PromoteToQueen:
                        queens[ColourToMoveIndex].RemovePieceAtSquare(movedTo);
                        break;
                    case Move.Flag.PromoteToKnight:
                        knights[ColourToMoveIndex].RemovePieceAtSquare(movedTo);
                        break;
                    case Move.Flag.PromoteToRook:
                        rooks[ColourToMoveIndex].RemovePieceAtSquare(movedTo);
                        break;
                    case Move.Flag.PromoteToBishop:
                        bishops[ColourToMoveIndex].RemovePieceAtSquare(movedTo);
                        break;
                }
            }
            else if (isEnPassant)
            {
                // ep cature: put captured pawn back on right square
                int epIndex = movedTo + ((ColourToMove == Piece.White) ? -8 : 8);
                Square[movedTo] = 0;
                Square[epIndex] = (int)capturedPiece;
                pawns[opponentColourIndex].AddPieceAtSquare(epIndex);
                ZobristKey ^= Zobrist.piecesArray[Piece.Pawn, opponentColourIndex, epIndex];
            }
            else if (moveFlags == Move.Flag.Castling)
            {
                // castles: move rook back to starting square

                bool kingside = movedTo == 6 || movedTo == 62;
                int castlingRookFromIndex = (kingside) ? movedTo + 1 : movedTo - 2;
                int castlingRookToIndex = (kingside) ? movedTo - 1 : movedTo + 1;

                Square[castlingRookToIndex] = 0;
                Square[castlingRookFromIndex] = Piece.Rook | ColourToMove;

                rooks[ColourToMoveIndex].MovePiece(castlingRookToIndex, castlingRookFromIndex);
                ZobristKey ^= Zobrist.piecesArray[Piece.Rook, ColourToMoveIndex, castlingRookFromIndex];
                ZobristKey ^= Zobrist.piecesArray[Piece.Rook, ColourToMoveIndex, castlingRookToIndex];
            }

            gameStateHistory.Pop(); // removes current state from history
            currentGameState = gameStateHistory.Peek(); // sets current state to previous state in history
            // Debug.LogError($"{gameStateHistory.Pop ()}");
            fiftyMoveCounter = (int)(currentGameState & 4294950912) >> 14;
            int newEnPassantFile = (int)(currentGameState >> 4) & 15;
            if (newEnPassantFile != 0)
                ZobristKey ^= Zobrist.enPassantFile[newEnPassantFile];

            uint newCastleState = currentGameState & 0b1111;
            if (newCastleState != originalCastleState)
            {
                ZobristKey ^= Zobrist.castlingRights[originalCastleState]; // remove old castling rights state
                ZobristKey ^= Zobrist.castlingRights[newCastleState]; // add new castling rights state
            }

            plyCount--;

            if (!inSearch && RepetitionPositionHistory.Count > 0)
            {
                RepetitionPositionHistory.Pop();
            }
        }

        // Load the starting position
        public void LoadStartPosition()
        {
            LoadPosition(FenUtility.startFen);
        }

        // Load custom position from fen string
        public void LoadPosition(string fen)
        {
            Initialize();
            var loadedPosition = FenUtility.PositionFromFen(fen);

            // Load pieces into board array and piece lists
            for (int squareIndex = 0; squareIndex < 64; squareIndex++)
            {
                int piece = loadedPosition.squares[squareIndex];
                Square[squareIndex] = piece;

                if (piece != Piece.None)
                {
                    int pieceType = Piece.PieceType(piece);
                    int pieceColourIndex = (Piece.IsColour(piece, Piece.White)) ? WhiteIndex : BlackIndex;
                    if (Piece.IsSlidingPiece(piece))
                    {
                        if (pieceType == Piece.Queen)
                        {
                            queens[pieceColourIndex].AddPieceAtSquare(squareIndex);
                        }
                        else if (pieceType == Piece.Rook)
                        {
                            rooks[pieceColourIndex].AddPieceAtSquare(squareIndex);
                        }
                        else if (pieceType == Piece.Bishop)
                        {
                            bishops[pieceColourIndex].AddPieceAtSquare(squareIndex);
                        }
                    }
                    else if (pieceType == Piece.Knight)
                    {
                        knights[pieceColourIndex].AddPieceAtSquare(squareIndex);
                    }
                    else if (pieceType == Piece.Pawn)
                    {
                        pawns[pieceColourIndex].AddPieceAtSquare(squareIndex);
                    }
                    else if (pieceType == Piece.King)
                    {
                        KingSquare[pieceColourIndex] = squareIndex;
                    }
                }
            }

            // Side to move
            WhiteToMove = loadedPosition.whiteToMove;
            ColourToMove = (WhiteToMove) ? Piece.White : Piece.Black;
            OpponentColour = (WhiteToMove) ? Piece.Black : Piece.White;
            ColourToMoveIndex = (WhiteToMove) ? 0 : 1;

            // Create gamestate
            int whiteCastle = ((loadedPosition.whiteCastleKingside) ? 1 << 0 : 0) |
                              ((loadedPosition.whiteCastleQueenside) ? 1 << 1 : 0);
            int blackCastle = ((loadedPosition.blackCastleKingside) ? 1 << 2 : 0) |
                              ((loadedPosition.blackCastleQueenside) ? 1 << 3 : 0);
            int epState = loadedPosition.epFile << 4;
            ushort initialGameState = (ushort)(whiteCastle | blackCastle | epState);
            gameStateHistory.Push(initialGameState);
            currentGameState = initialGameState;
            plyCount = loadedPosition.plyCount;

            // Initialize zobrist key
            ZobristKey = Zobrist.CalculateZobristKey(this);
        }

        void Initialize()
        {
            Square = new int[64];
            KingSquare = new int[2];

            gameStateHistory = new Stack<uint>();
            ZobristKey = 0;
            RepetitionPositionHistory = new Stack<ulong>();
            plyCount = 0;
            fiftyMoveCounter = 0;

            knights = new PieceList[] { new PieceList(10), new PieceList(10) };
            pawns = new PieceList[] { new PieceList(8), new PieceList(8) };
            rooks = new PieceList[] { new PieceList(10), new PieceList(10) };
            bishops = new PieceList[] { new PieceList(10), new PieceList(10) };
            queens = new PieceList[] { new PieceList(9), new PieceList(9) };
            PieceList emptyList = new PieceList(0);
            allPieceLists = new PieceList[]
            {
                emptyList,
                emptyList,
                pawns[WhiteIndex],
                knights[WhiteIndex],
                emptyList,
                bishops[WhiteIndex],
                rooks[WhiteIndex],
                queens[WhiteIndex],
                emptyList,
                emptyList,
                pawns[BlackIndex],
                knights[BlackIndex],
                emptyList,
                bishops[BlackIndex],
                rooks[BlackIndex],
                queens[BlackIndex],
            };
        }
    }
}