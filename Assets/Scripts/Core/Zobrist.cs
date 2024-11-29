using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Chess {
	public static class Zobrist {
		const int seed = 2361912;
		const string randomNumbersFileName = "RandomNumbers.txt";

		public static readonly ulong[,,] piecesArray = new ulong[8, 2, 64];
		public static readonly ulong[] castlingRights = new ulong[16];
		public static readonly ulong[] enPassantFile = new ulong[9];
		public static ulong sideToMove;

		static System.Random prng = new System.Random(seed);

		static void WriteRandomNumbers() {
			prng = new System.Random(seed);
			string randomNumberString = "";
			int numRandomNumbers = 64 * 8 * 2 + castlingRights.Length + 9 + 1;

			for (int i = 0; i < numRandomNumbers; i++) {
				randomNumberString += RandomUnsigned64BitNumber();
				if (i != numRandomNumbers - 1) {
					randomNumberString += ',';
				}
			}
			var writer = new StreamWriter(randomNumbersPath);
			writer.Write(randomNumberString);
			writer.Close();
		}

		static IEnumerator ReadRandomNumbers(Action<Queue<ulong>> onComplete) {
			Queue<ulong> randomNumbers = new Queue<ulong>();

			string filePath = Path.Combine(Application.streamingAssetsPath, randomNumbersFileName);
			
			#if UNITY_ANDROID
			using (UnityWebRequest www = UnityWebRequest.Get(filePath)) {
				yield return www.SendWebRequest();

				if (www.result != UnityWebRequest.Result.Success) {
					Debug.LogError("Failed to load file: " + www.error);
				} else {
					string numbersString = www.downloadHandler.text;
					string[] numberStrings = numbersString.Split(',');

					foreach (string numStr in numberStrings) {
						randomNumbers.Enqueue(ulong.Parse(numStr));
					}
				}
			}
			#else
			if (!File.Exists(filePath)) {
				WriteRandomNumbers();
			}
			
			using (var reader = new StreamReader(filePath)) {
				string numbersString = reader.ReadToEnd();
				string[] numberStrings = numbersString.Split(',');

				foreach (string numStr in numberStrings) {
					randomNumbers.Enqueue(ulong.Parse(numStr));
				}
			}
			#endif

			onComplete(randomNumbers);
		}

		static Zobrist() {
			GameObject obj = new GameObject("ZobristInitializer");
			obj.AddComponent<ZobristInitializer>();
		}

		public static ulong CalculateZobristKey(Board board) {
			ulong zobristKey = 0;

			for (int squareIndex = 0; squareIndex < 64; squareIndex++) {
				if (board.Square[squareIndex] != 0) {
					int pieceType = Piece.PieceType(board.Square[squareIndex]);
					int pieceColour = Piece.Colour(board.Square[squareIndex]);

					zobristKey ^= piecesArray[pieceType, (pieceColour == Piece.White) ? Board.WhiteIndex : Board.BlackIndex, squareIndex];
				}
			}

			int epIndex = (int)(board.currentGameState >> 4) & 15;
			if (epIndex != -1) {
				zobristKey ^= enPassantFile[epIndex];
			}

			if (board.ColourToMove == Piece.Black) {
				zobristKey ^= sideToMove;
			}

			zobristKey ^= castlingRights[board.currentGameState & 0b1111];

			return zobristKey;
		}

		static string randomNumbersPath {
			get {
				return Path.Combine(Application.streamingAssetsPath, randomNumbersFileName);
			}
		}

		static ulong RandomUnsigned64BitNumber() {
			byte[] buffer = new byte[8];
			prng.NextBytes(buffer);
			return BitConverter.ToUInt64(buffer, 0);
		}

		public class ZobristInitializer : MonoBehaviour {
			void Start() {
				StartCoroutine(ReadRandomNumbers(randomNumbers => {
					var randNumArray = randomNumbers.ToArray();
					int index = 0;

					for (int squareIndex = 0; squareIndex < 64; squareIndex++) {
						for (int pieceIndex = 0; pieceIndex < 8; pieceIndex++) {
							piecesArray[pieceIndex, Board.WhiteIndex, squareIndex] = randNumArray[index++];
							piecesArray[pieceIndex, Board.BlackIndex, squareIndex] = randNumArray[index++];
						}
					}

					for (int i = 0; i < 16; i++) {
						castlingRights[i] = randNumArray[index++];
					}

					for (int i = 0; i < enPassantFile.Length; i++) {
						enPassantFile[i] = randNumArray[index++];
					}

					sideToMove = randNumArray[index];
					Destroy(gameObject);
				}));
			}
		}
	}
}
