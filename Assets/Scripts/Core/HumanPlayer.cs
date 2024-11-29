using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Game
{
    public class HumanPlayer : Player
    {
        public enum InputState
        {
            None,
            PieceSelected,
            DraggingPiece
        }

        InputState currentState;

        BoardUI boardUI;
        Camera cam;
        Coord selectedPieceSquare;
        Coord dutPieceSquare;
        Board board;

        //todo: TINH phần này khai báo cho phần bên dưới
        // private int whiteKnightCounter = 1, blackKnightCounter = 1;
        // private int whitePawnCounter = 1, blackPawnCounter = 1;
        // private int whiteBishopCounter = 1, blackBishopCounter = 1;
        // private int whiteRookCounter = 1, blackRookCounter = 1;
        //
        // private Dictionary<int, string> pieceIdentifiers = new Dictionary<int, string>();
        // private HashSet<string> movedPieces = new HashSet<string>();
        

        public HumanPlayer(Board board)
        {
            boardUI = GameObject.FindObjectOfType<BoardUI>();
            cam = Camera.main;
            this.board = board;
        }

        public override void NotifyTurnToMove()
        {
        }

        public override void Update()
        {
            HandleInput();
        }

        void HandleInput()
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            if (currentState == InputState.None)
            {
                HandlePieceSelection(mousePos);
            }
            else if (currentState == InputState.DraggingPiece)
            {
                HandleDragMovement(mousePos);
            }
            else if (currentState == InputState.PieceSelected)
            {
                HandlePointAndClickMovement(mousePos);
            }

            if (Input.GetMouseButtonDown(1))
            {
                CancelPieceSelection();
            }
        }

        void HandlePointAndClickMovement(Vector2 mousePos)
        {
            if (Input.GetMouseButton(0))
            {
                HandlePiecePlacement(mousePos);
            }
        }

        void HandleDragMovement(Vector2 mousePos)
        {
            boardUI.DragPiece(selectedPieceSquare, mousePos);
            // If mouse is released, then try place the piece
            if (Input.GetMouseButtonUp(0))
            {
                HandlePiecePlacement(mousePos);
            }
        }

        void HandlePiecePlacement(Vector2 mousePos)
        {
            Coord targetSquare;
            if (boardUI.TryGetSquareUnderMouse(mousePos, out targetSquare))
            {
                if (targetSquare.Equals(selectedPieceSquare))
                {
                    boardUI.ResetPiecePosition(selectedPieceSquare);
                    if (currentState == InputState.DraggingPiece)
                    {
                        currentState = InputState.PieceSelected;
                    }
                    else
                    {
                        currentState = InputState.None;
                        boardUI.DeselectSquare(selectedPieceSquare);
                    }
                }
                else
                {
                    int targetIndex =
                        BoardRepresentation.IndexFromCoord(targetSquare.fileIndex, targetSquare.rankIndex);
                    if (Piece.IsColour(board.Square[targetIndex], board.ColourToMove) && board.Square[targetIndex] != 0)
                    {
                        CancelPieceSelection();
                        HandlePieceSelection(mousePos);
                    }
                    else
                    {
                        TryMakeMove(selectedPieceSquare, targetSquare);
                    }
                }
            }
            else
            {
                CancelPieceSelection();
            }
        }

        void CancelPieceSelection()
        {
            if (currentState != InputState.None)
            {
                currentState = InputState.None;
                boardUI.DeselectSquare(selectedPieceSquare);
                boardUI.ResetPiecePosition(selectedPieceSquare);
            }
        }

        void TryMakeMove(Coord startSquare, Coord targetSquare)
        {
            dutPieceSquare = targetSquare;
            int startIndex = BoardRepresentation.IndexFromCoord(startSquare);
            int targetIndex = BoardRepresentation.IndexFromCoord(targetSquare);
            Debug.Log($"Try Make Move: {startSquare} - {targetSquare} - {startIndex} - {targetIndex}");
            bool moveIsLegal = false;
            Move chosenMove = new Move();

            MoveGenerator moveGenerator = new MoveGenerator();
            bool wantsKnightPromotion = Input.GetKey(KeyCode.LeftAlt);

            var legalMoves = moveGenerator.GenerateMoves(board);
            for (int i = 0; i < legalMoves.Count; i++)
            {
                var legalMove = legalMoves[i];

                if (legalMove.StartSquare == startIndex && legalMove.TargetSquare == targetIndex)
                {
                    if (legalMove.IsPromotion)
                    {
                        if (legalMove.MoveFlag == Move.Flag.PromoteToQueen && wantsKnightPromotion)
                        {
                            continue;
                        }

                        if (legalMove.MoveFlag != Move.Flag.PromoteToQueen && !wantsKnightPromotion)
                        {
                            continue;
                        }
                    }

                    moveIsLegal = true;
                    chosenMove = legalMove;
                    //	Debug.Log (legalMove.PromotionPieceType);
                    break;
                }
            }

            if (moveIsLegal)
            {
                var dutBlind = new DutBlind(dutPieceSquare.fileIndex, dutPieceSquare.rankIndex);
                if (!BlindChessController.instance.dutBlinds.Contains(dutBlind))
                {
                    BlindChessController.instance.dutBlinds.Add(dutBlind);
                }
                ChoseMove(chosenMove);
                OnMoveDone(chosenMove.TargetSquare);
                currentState = InputState.None;
            }
            else
            {
                CancelPieceSelection();
            }
        }

        void HandlePieceSelection(Vector2 mousePos)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (boardUI.TryGetSquareUnderMouse(mousePos, out selectedPieceSquare))
                {
                    int index = BoardRepresentation.IndexFromCoord(selectedPieceSquare);

                    // Debug.Log($"select piece: {BoardRepresentation.SquareNameFromCoordinate(selectedPieceSquare.fileIndex, selectedPieceSquare.rankIndex)}");
                    // Debug.Log($"select piece: {BoardRepresentation.SquareNameFromIndex(index)}");
                    // Debug.Log($"select piece: {BoardRepresentation.SquareNameFromCoordinate(selectedPieceSquare)}");
                    // If square contains a piece, select that piece for dragging
                    if (Piece.IsColour(board.Square[index], board.ColourToMove))
                    {
                        PrintSelectedPiece(index);
                        boardUI.HighlightLegalMoves(board, selectedPieceSquare);
                        boardUI.SelectSquare(selectedPieceSquare);
                        currentState = InputState.DraggingPiece;
                        //TODO:CLickSelectedPiece
                        //White:
                        // xe: 14
                        // ma: 11
                        // tuong: 13
                        // hau: 15
                        // vua: 9
                        // tot: 10

                        //Black:
                        // xe: 22
                        // ma: 19
                        // tuong: 21
                        // hau: 23
                        // vua: 17
                        // tot: 18
                    }
                }
            }
        }

        private string pieceName;
        private int currentIndex;

        void PrintSelectedPiece(int index)
        {
            pieceName = "Unknown";
            currentIndex = index;
            int pieceType = board.Square[index];
            List<int> pieceIndexes = new List<int>();

            for (int i = 0; i < board.Square.Length; i++)
            {
                if (board.Square[i] == pieceType)
                {
                    pieceIndexes.Add(i);
                }
            }

            switch (pieceType)
            {
                case 10: pieceName = "WhitePawn"; break; // Tốt
                case 11: pieceName = "WhiteKnight"; break; // Mã
                case 13: pieceName = "WhiteBishop"; break; // Tượng
                case 14: pieceName = "WhiteRook"; break; // Xe
                case 15: pieceName = "WhiteQueen"; break; // Hậu
                case 9: pieceName = "WhiteKing"; break; // Vua

                case 18: pieceName = "BlackPawn"; break;
                case 19: pieceName = "BlackKnight"; break;
                case 21: pieceName = "BlackBishop"; break;
                case 22: pieceName = "BlackRook"; break;
                case 23: pieceName = "BlackQueen"; break;
                case 17: pieceName = "BlackKing"; break;
                default:
                    pieceName = "Unknown piece";
                    break;
            }
            
            // 0 - 15 and 49 - 63 except 4 and 60

            if (BlindChessController.instance.PieceSelected.ContainsKey(currentIndex))
            {
                // Debug.Log($"Selecting Piece {pieceName} - {index} - {BlindChessController.instance.PieceSelected[currentIndex]}");
                PlayerPrefs.SetString("CurrentPiece", $"{pieceName}_{currentIndex}");
            }
            else
            {
                // Debug.Log($"Selecting Piece {pieceName}");
                PlayerPrefs.SetString("CurrentPiece", $"{pieceName}_{-1}");
            }
            
        }

        public void OnMoveDone(int moveTo)
        { 
            //todo: TINH phần này check xem đã nhấc lên chưa rồi lưu lại

            if (BlindChessController.instance.PieceSelected.ContainsKey(currentIndex))
            {
                BlindChessController.instance.PieceSelected[currentIndex] = false;
                Debug.Log($"On Move Done {currentIndex} - {moveTo} - {board.Square[moveTo]}");
                boardUI.UpdateBLindChess(dutPieceSquare, board.Square[moveTo]);
            }
        }
    }
}