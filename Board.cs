
using System;
using System.Collections.Generic;
using System.Linq;

namespace Joueur.cs.Games.Chess.Logic
{

    class Board
    {
        public Cell KingCell {get;set;}
        public Cell opponentKingCell {get;set;}
        public List<Cell> KingCheckmatePieces {get;set;}
        public int Size {get;set;}
        public HashSet<Cell> WhitePieces {get;set;}
        public Dictionary<char, int> deadWhitePieces {get;set;}
        public HashSet<Cell> BlackPieces {get;set;}
        public Dictionary<char, int> deadBlackPieces {get;set;}
        //public HashSet<string> cellsopponentCanMoveTo {get;set;}
        public int nextToMove {get;set;} //-1 Black || 1 White
        public bool[] castling = new bool[4];
        public string enpassant {get;set;}
        public int halfmove {get;set;}  //not used****************************
        public int fullmove {get;set;}  //not used****************************
        //2d array
        public Cell[,] grid {get;set;} //row,col
        
        //scores-------
        public int BlackScore;
        public int WhiteScore;
        //----------------------
        public List<string> moveHistory;
        public string fenstring;
        //moves that this board can generate by nextToMove player
        public List<string> moves_available;

         
         //these are used for sliding_addmoves for the forloop check
        public static Func<int, int, bool> forcheckCritera_negative= (A1,A2)=> A1>=A2; 
        public static Func<int, int, bool> forcheckCritera_positive= (A1,A2)=> A1<A2;

        public MoveType lastMovedone;
        
        /*  Brief: Contrustructor
        *   @param[in] s board size to be constructed (yeah only do 8)
        *   @pre s should be 8x8 for this to work
        *   @post makes an 8x8 board and constructs board
        */
        public Board (int s)
        {
            deadWhitePieces = new Dictionary<char, int>();
            deadBlackPieces = new Dictionary<char, int>();
            //cellsopponentCanMoveTo = new HashSet<string>();
            //kingisCurrentlyInCheckMate=false;
            KingCell = new Cell();
            opponentKingCell = new Cell();
            
            WhitePieces = new HashSet<Cell>();
            BlackPieces = new HashSet<Cell>();
            KingCheckmatePieces = new List<Cell>();
            moveHistory = new List<string>();
            moves_available = new List<string>();
            Size = s;
            grid = new Cell [Size,Size];
            //fill grid with x and y coordinates
            for (int y =0; y<Size;y++)
            {
                for (int x =0; x<Size;x++)
                {
                    grid[y,x] = new Cell(x,y);
                }
            }
        }
        
        /*  Brief: Copy Constructor
        *   @param[in] b board to be copied.
        *   @post makes a new copy of board from board b
        */
        public Board(Board b) //update listings
        {
            
            Size = b.Size;
            
            //generatedOpponentMove = b.generatedOpponentMove;
            //kingisCurrentlyInCheckMate = b.kingisCurrentlyInCheckMate;
            KingCell = new Cell(b.KingCell);
            opponentKingCell = new Cell(b.opponentKingCell);

            //this only gets cleared whenever a king is in checkmater
            //just prevent ref
            KingCheckmatePieces = new List<Cell>(); 

            Size = b.Size;
            WhitePieces = new HashSet<Cell>(b.WhitePieces);
            deadWhitePieces = new  Dictionary<char, int> (b.deadWhitePieces);
            BlackPieces = new  HashSet<Cell> (b.BlackPieces);
            deadBlackPieces = new  Dictionary<char, int> (b.deadBlackPieces);
            //cellsopponentCanMoveTo = new HashSet<string> (b.cellsopponentCanMoveTo);
            nextToMove = b.nextToMove;

            castling = new bool[4];
            Array.Copy(b.castling, castling, 4);

            enpassant = b.enpassant;
            halfmove = b.halfmove;
            fullmove = b.fullmove;

            grid = new Cell[Size, Size];
            Array.Copy(b.grid, grid, Size*Size);

            BlackScore= b.BlackScore;
            WhiteScore= b.WhiteScore;
            moveHistory= new List<string>(b.moveHistory);
            moves_available = new List<string>(b.moves_available);
        }


        /*  Brief: Make Move Function
        *   @param[in] move_action move in the form of initialcell and final cell
        *   (ex) "e5c5","b2d4","g7g8P" etc...
        *   @pre board has a playable state (updateBoard(string) is called at least once)
        *   @pre Assumes move_action is a valid move
        *   @post changes the board based on move_action
        *   @post updates score if applicable
        */
        //ASSUMES MOVE IS VALID
        public void MakeMove(string move_action)
        {
            //setup initialization of this before moving
            halfmove++;
            lastMovedone=MoveType.NOTHING;
            moveHistory.Add(move_action);
            char[] chars = move_action.ToCharArray ();
            string s_part1=(chars[0].ToString()+chars[1].ToString());
            string s_part2=(chars[2].ToString()+chars[3].ToString());

            //RETURNS col,row [0],[1]
            //currentlocation
            List<int> initial=Cell.convertLocation(s_part1);
            //movetolocation
            List<int> final=Cell.convertLocation(s_part2);
            

            Cell c_cell= grid[initial[1],initial[0]];
            Cell f_cell= grid[final[1],final[0]];
            
            bool movedKing=false;
            //----------------------------

            //CHECK IF non enpassant, then erase to "-"
            enpassant="-";

            //Update Score/Pieces
            if(nextToMove==(int)TURN.BLACK) //black
            {
                //CHECK edge cases
                if(c_cell.location==KingCell.location) //CHECK IF CASTLING
                {
                    //MOVE THE ROOKS
                    //KingSide
                    if(c_cell.location=="e8" && f_cell.location=="g8" && castling[2]==true)
                    {

                        BlackPieces.Remove(grid[0,7]);

                        grid[0,5] = new Cell(grid[0,7]); //move
                        BlackPieces.Add(grid[0,5]);

                        grid[0,7]=Cell.emptyCell(7,0); //remove    
                        
                    }
                    else if(c_cell.location=="e8" && f_cell.location=="c8" && castling[3]==true)
                    {
                        BlackPieces.Remove(grid[0,0]);           

                        grid[0,3] = new Cell(grid[0,0]); //move
                        BlackPieces.Add(grid[0,3]);

                        grid[0,0]=Cell.emptyCell(0,0); //remove      
                    }
                    movedKing=true;
                    castling[2]=false;
                    castling[3]=false;
                }
                else if(c_cell.piece.name == 'p')//check if moved forward // check pawn
                {
                    if(enpassant==(s_part2)) //pawnmove
                    {
                        UpdateScore(TURN.WHITE,(-(int)PIECEVALUE.PAWN),MoveType.CAPTURE);
                    }
                    else if(initial[1] == 0 && final[1] == 3) //initial row
                    {
                        enpassant=(chars[2].ToString()+'6');                        
                    }
                    halfmove=0;
                }
                else if(c_cell.piece.name == 'r') //undo castle
                {
                    if(c_cell.location == "h8") //kingside
                    {
                        castling[2]=false;
                    }
                    else if(c_cell.location == "a8")
                    {
                        castling[3]=false;
                    }
                }

                //******************************************
                //--UpdatePieces
                //update deadpieces/ //CAPTURE NORMALLY
                if(!(f_cell.isEmpty())) //not empty
                {
                    UpdateScore(TURN.WHITE,(-f_cell.piece.value),MoveType.CAPTURE);
                    WhitePieces.Remove(f_cell);              
                    deadWhitePieces[f_cell.piece.name] = deadWhitePieces[f_cell.piece.name]+1;
                    halfmove=0;
                }
            }
            else
            {
                if(c_cell.location==KingCell.location) //CHECK IF CASTLING
                {
                    //MOVE THE ROOKS
                    //KingSide
                    if(c_cell.location=="e1" && f_cell.location=="g1" && castling[0]==true)
                    {
                        WhitePieces.Remove(grid[7,7]);

                        grid[7,5] = new Cell(grid[7,7]);                        
                        WhitePieces.Add(grid[7,5]);

                        grid[7,7]=Cell.emptyCell(7,7); //remove
                        //preventCastling
                    }
                    else if(c_cell.location=="e1" && f_cell.location=="c1" && castling[1]==true) //queenside
                    {
                        WhitePieces.Remove(grid[7,0]);

                        grid[7,3] = new Cell(grid[7,0]); //move              
                        WhitePieces.Add(grid[7,3]);

                        grid[7,0]=Cell.emptyCell(0,7); //remove
                    }
                    movedKing=true;
                    castling[0]=false;
                    castling[1]=false;
                }                
                else if(c_cell.piece.name == 'P')//check if moved forward // check pawn
                {                    
                    if(enpassant==(s_part2)) //pawnmove
                    {
                        UpdateScore(TURN.BLACK,(-(int)PIECEVALUE.PAWN),MoveType.CAPTURE);
                    }
                    else if(initial[1] == 6 && final[1] == 4) //initial row //moved twice
                    {
                        enpassant=(chars[2].ToString()+3);
                    }
                    halfmove=0;
                }
                else if(c_cell.piece.name == 'R') //undo castle
                {
                    if(c_cell.location == "h1") //kingside
                    {
                        castling[0]=false;
                    }
                    else if(c_cell.location == "a1") //queenside
                    {
                        castling[1]=false;
                    }
                }
                //******************************************
                if(!(f_cell.isEmpty())) //not empty
                {                    
                    UpdateScore(TURN.BLACK,(-f_cell.piece.value),MoveType.CAPTURE);
                    BlackPieces.Remove(f_cell);
                    deadBlackPieces[f_cell.piece.name] = deadBlackPieces[f_cell.piece.name]+1;
                    halfmove=0;
                }
            }

            if(nextToMove==(int)TURN.BLACK)
            {
                BlackPieces.Remove(grid[initial[1],initial[0]]);
            }
            else
            {
                WhitePieces.Remove(grid[initial[1],initial[0]]);
            }
            //replace cell-------------------------------------------
            if(chars.Count()==5) //promote to pawn to given
            {
                Cell new_cell= new Cell(c_cell,final[0],final[1]);
                grid[final[1],final[0]]=new_cell;

                if(nextToMove==(int)TURN.BLACK) //back
                {
                    char lower= char.ToLower(chars[4]);
                    grid[final[1],final[0]].placePiece(lower);
                    BlackPieces.Add(grid[final[1],final[0]]);
                    
                    UpdateScore(TURN.BLACK,(grid[final[1],final[0]].piece.value),MoveType.PROMOTION);


                    deadBlackPieces[lower] = deadBlackPieces[lower]-1;
                }
                else
                {
                    char upper= char.ToUpper(chars[4]);
                    grid[final[1],final[0]].placePiece(upper);
                    WhitePieces.Add(grid[final[1],final[0]]);

                    UpdateScore(TURN.WHITE,(grid[final[1],final[0]].piece.value),MoveType.PROMOTION);

                    deadWhitePieces[upper] = deadWhitePieces[upper]-1;
                }                                
            }
            else
            {
                Cell new_cell= new Cell(c_cell,final[0],final[1]);
                grid[final[1],final[0]]=new_cell;
            }
            
            if(nextToMove==(int)TURN.BLACK)
            {
                BlackPieces.Add(grid[final[1],final[0]]);
            }
            else
            {
                WhitePieces.Add(grid[final[1],final[0]]);
            }

            if(movedKing)
            {
                KingCell= grid[final[1],final[0]];
            }
            //remove old cell
            grid[initial[1],initial[0]] =  Cell.emptyCell(initial[0],initial[1]);
    
            //turnoverplayer
            nextToMove=-nextToMove;

            Cell temp=new Cell(KingCell);
            KingCell=new Cell(opponentKingCell);
            opponentKingCell=new Cell(temp);

            if(nextToMove==(int)TURN.BLACK)
            {
                fullmove++;
            }
        }

        public bool isQuiescence()
        {
            foreach(String s_move in moves_available)
            {
                MoveType moveType= checkMoveType(s_move);
                if(moveType == MoveType.CAPTURE || moveType ==MoveType.PROMOTION)
                {
                    return false;                
                }
            }
            return true;
        }
        
        /*  Brief: checkMoveType
        *   @param[in] move_action to check action type
        *   @pre move_action must be valid
        *   @pre Board must be updated
        *   @post updates score based on parameters, sets lastmoveDone to move given
        */
        public MoveType checkMoveType(string move_action)
        {
            //setup initialization of this before moving
            char[] chars = move_action.ToCharArray ();
            string s_part1=(chars[0].ToString()+chars[1].ToString());
            string s_part2=(chars[2].ToString()+chars[3].ToString());

            //RETURNS col,row [0],[1]
            //currentlocation
            List<int> initial=Cell.convertLocation(s_part1);
            //movetolocation
            List<int> final=Cell.convertLocation(s_part2);
            

            Cell c_cell= grid[initial[1],initial[0]];
            Cell f_cell= grid[final[1],final[0]];
            
            //----------------------------

            //CheckCaptures
            if(c_cell.piece.name == 'p' || c_cell.piece.name == 'P')//check if moved forward // check pawn
            {
                if(enpassant==(s_part2)) //pawnmove
                {
                    return MoveType.CAPTURE;
                }
            }
            //CAPTURE NORMALLY
            if(!(f_cell.isEmpty())) //not empty
            {
                return MoveType.CAPTURE;
            }
        
            //replace cell-------------------------------------------
            if(chars.Count()==5) //promote to pawn to given
            {
                return MoveType.PROMOTION;
            }

            return MoveType.NOTHING;
        }

        /*  Brief: UpdatesScore
        *   @param[in] player which to update score
        *   @param[in] scoretoAdd how much to change score (can be -/+)
        *   @param[in] moveDone move that made an update to score (for future use)
        *   @pre player must be in range of TURN enum
        *   @pre moveDone must be in range of MoveType enum
        *   @post updates score based on parameters, sets lastmoveDone to move given
        */
        public void UpdateScore(TURN player, int scoretoAdd,MoveType moveDone)
        {
            if(moveDone == MoveType.CAPTURE || moveDone == MoveType.PROMOTION)
            {
                if(player == TURN.WHITE)
                {
                    WhiteScore+=scoretoAdd;

                }
                else if (player == TURN.BLACK)
                {
                    BlackScore+=scoretoAdd;
                }
            }
            lastMovedone=moveDone;

        }
        /*  Brief: CreateFen
        *   @pre board must be in a playable state
        *   @post Updates FENstring of this current board.
        */
        public void CreateFEN()
        {
            fenstring="";
            for(int y=0;y<Size;y++)
            {
                int spaces=0;
                for(int x=0;x<Size;x++)
                {
                    Cell s_cell=grid[y,x];
                    if(s_cell.CurrentlyOccupied)
                    {
                        if(spaces>0)
                        {
                            fenstring+=spaces;
                        }
                        fenstring+=s_cell.piece.name;
                        spaces=0;
                    }
                    else
                    {
                        spaces++;                        
                    }
                }                
                if(spaces>0)
                {
                    fenstring+=spaces;
                }
                if(y<Size-1)
                {
                    fenstring+="/";
                }
            }
            fenstring+=" ";
            fenstring+= (nextToMove==(int)TURN.BLACK) ? "b":"w";
            fenstring+=" ";
            if(castling[0] || castling[1] || castling[2] || castling[3])
            {
                if(castling[0]) //Skip Lines
                {
                    fenstring+='K';
                }
                if(castling[1])
                {
                    fenstring+='Q';
                }
                if(castling[2]) //black
                {
                    fenstring+='k';
                }
                if(castling[3]) //black
                {
                    fenstring+='q';
                }
            }
            else
            {
                fenstring+="-";
            }
            fenstring+=" ";
            fenstring+=enpassant;
            fenstring+=" ";
            fenstring+= halfmove;
            fenstring+=" ";
            fenstring+= fullmove;

        }
        /*  Brief: CreateFen
        *   @param[in] player calculates score in favor of this parameter
        *   if black is called, a positive score means black is winning. (vice versa)
        *   @pre player is -1 or 1
        *   @post returns score based on the boardstate
        */
        public int calculateScore(int player) //-1 or 1
        {
            int score=0;
            int tempscore=0;
            
            if(player==(int)TURN.BLACK) //Black>0 is good
            {
                score+=BlackScore-WhiteScore;
                tempscore=score;
                if(nextToMove==(int)TURN.BLACK)
                {
                    //is black Checkmated?
                    score-= (isCheckmate(KingCell))? (int)PIECEVALUE.KING : 0;             
                }
                else
                {
                    //is white Checkmated?
                    score+= (isCheckmate(KingCell))? (int)PIECEVALUE.KING : 0;
                }
            }
            else//White>0 is good
            {
                score+=WhiteScore-BlackScore;
                
                if(nextToMove==(int)TURN.BLACK)
                {
                    //is black Checkmated?
                    score+= (isCheckmate(KingCell))? (int)PIECEVALUE.KING : 0;
                }
                else
                {
                    //is white Checkmated?
                    score-= (isCheckmate(KingCell))? (int)PIECEVALUE.KING : 0;
                }
            }
            return score;
        }
        
        /*  Brief: Reset deadPieces
        *   @pre board must be constructed and updated by fenstring
        *   @post resets dead pieces back to all dead.
        */
        public void resetDeadPieces()
        {
            deadBlackPieces.Clear();     
            deadWhitePieces.Clear();
            deadWhitePieces = new Dictionary<char, int>()
            {
                { 'P', 8 },
                { 'N', 2 },
                { 'B', 2 },
                { 'R', 2 },
                { 'Q', 1 },
                { 'K', 1 }
            };
            deadBlackPieces = new Dictionary<char, int>()
            {
                { 'p', 8 },
                { 'n', 2 },
                { 'b', 2 },
                { 'r', 2 },
                { 'q', 1 },
                { 'k', 1 }
            };
        }


        /*  Brief: Returns moves that can be done from board 
        *   @param[in] m mode to use (random selects a random piece) (select selects all pieces)
        *   @pre board must be fully updated
        *   @post returns moves that can be done from the board for the current player to move (nextToMove)
        */
        public List<string> move_select(Mode m)
        {
            moves_available = new List<string>();
            if(nextToMove==(int)TURN.BLACK) //black's turn
            {
                if(m.Equals(Mode.random))
                {
                    int selectedPiece= RNG.GenerateRandomNumber(0,BlackPieces.Count()-1);    
                    Cell[] BlackPiecessasArray = BlackPieces.ToArray();
                    moves_available.AddRange(returnMovesFromCell(BlackPiecessasArray[selectedPiece]));     
                }
                else
                {
                    KingCheckmatePieces.Clear();
                    if(!isCheckmate(KingCell))
                    {
                        //return moves from all possible pieces to move
                        foreach(var p in BlackPieces)
                        {
                            moves_available.AddRange(returnMovesFromCell(p));
                        }
                    }
                    else //king has to be safe,make a move to make the king safe
                    {
                        moves_available.AddRange(returnMovesFromCell(KingCell)); //moves from king

                        List<string> cellstomoveOn = new List<string>();
                        cellstomoveOn.Add(KingCheckmatePieces[0].location);

                        
                        //moves to elimate checkmatePiece
                        List<string> tempList = new List<string>();
                        foreach(var p in BlackPieces)
                        {
                            if(p.location!=KingCell.location)
                                tempList.AddRange(returnMovesFromCell(p));
                        }

                        //cells in the way of king //these are bishop and rooks
                        if(KingCheckmatePieces[0].piece.name == 'B')
                        {
                            cellstomoveOn.AddRange(findpathtoKingBishop(KingCheckmatePieces[0]));
                        }
                        else if(KingCheckmatePieces[0].piece.name == 'R')
                        {
                            cellstomoveOn.AddRange(findpathtoKingRook(KingCheckmatePieces[0]));
                        }
                        else if(KingCheckmatePieces[0].piece.name == 'Q')
                        {
                            cellstomoveOn.AddRange(findpathtoKingBishop(KingCheckmatePieces[0]));
                            cellstomoveOn.AddRange(findpathtoKingRook(KingCheckmatePieces[0]));
                        }
                        //-----------------
                        //eliminate moves that are not going to make king safe
                        foreach(string s in tempList)
                        {
                            foreach(string piecetoEliminate in cellstomoveOn)
                            {
                                if(s.Contains(piecetoEliminate))
                                {
                                    moves_available.Add(s);
                                    break;
                                }    
                            }
                        }
                    }
                }
            }
            else
            {
                if(m.Equals(Mode.random))
                {
                    int selectedPiece= RNG.GenerateRandomNumber(0,WhitePieces.Count()-1);
                    Cell[] WhitePiecesasArray = BlackPieces.ToArray();
                    moves_available.AddRange(returnMovesFromCell(WhitePiecesasArray[selectedPiece]));     
                }
                else
                {
                    KingCheckmatePieces.Clear();
                    if(!isCheckmate(KingCell))
                    {
                        //return moves from all possible pieces to move
                        foreach(var p in WhitePieces)
                        {
                            
                            moves_available.AddRange(returnMovesFromCell(p));
                        }
                    }
                    else //king has to be safe,make a move to make the king safe
                    {
                        moves_available.AddRange(returnMovesFromCell(KingCell)); //moves from king

                        List<string> cellstomoveOn = new List<string>();
                        cellstomoveOn.Add(KingCheckmatePieces[0].location);

                        //moves to elimate checkmatePiece
                        List<string> tempList = new List<string>();
                        foreach(var p in WhitePieces)
                        {
                            if(p.location!=KingCell.location)
                                tempList.AddRange(returnMovesFromCell(p));
                        }

                        //cells in the way of king //these are bishop and rooks
                        //-------------------------------
                        if(KingCheckmatePieces[0].piece.name == 'b')
                        {
                            cellstomoveOn.AddRange(findpathtoKingBishop(KingCheckmatePieces[0]));
                        }
                        else if(KingCheckmatePieces[0].piece.name == 'r')
                        {
                            cellstomoveOn.AddRange(findpathtoKingRook(KingCheckmatePieces[0]));
                        }
                        else if(KingCheckmatePieces[0].piece.name == 'q')
                        {
                            cellstomoveOn.AddRange(findpathtoKingBishop(KingCheckmatePieces[0]));
                            cellstomoveOn.AddRange(findpathtoKingRook(KingCheckmatePieces[0]));
                        }
                        //--------------
                        //eliminate moves that are not going to make king safe
                        foreach(string s in tempList)
                        {
                            foreach(string piecetoEliminate in cellstomoveOn)
                            {
                                if(s.Contains(piecetoEliminate))
                                {
                                    moves_available.Add(s);
                                    break;
                                }    
                            }
                        }
                    }
                }
            }
            return moves_available;
        }
        
        /*  Brief: Moves that can be done from cell selected
        *   @param[in] c_cell sell to select
        *   @pre (board should be updated or returns nothing)
        *   @post returns moves that can be done from cell. If piece exists returns piece moves.
        */
        public List<string> returnMovesFromCell ( Cell c_cell)
        {
            List<string> toReturn= new List<string>();


            switch (Char.ToLower(c_cell.piece.name))
            {
                case 'p':
                toReturn.AddRange(PawnMoves(ref c_cell));
                    break;
                case 'n':
                toReturn.AddRange(KnightMoves(ref c_cell));
                    break;
                case 'k':
                toReturn.AddRange(KingMoves(ref c_cell));
                    break;
                case 'r':
                toReturn.AddRange( RookMoves(ref c_cell));
                    break;
                case 'b':
                toReturn.AddRange(BishopMoves(ref c_cell));
                    break;
                case 'q':
                toReturn.AddRange(BishopMoves(ref c_cell));
                toReturn.AddRange(RookMoves(ref c_cell));
                    break;
                default:
                //no moves to return
                    break;
            }
            for(int i=0;i<toReturn.Count();i++)
            {
                toReturn[i]= c_cell.location+toReturn[i];
            }
            return toReturn;
        }

        /*  Brief: CHECKS CELL IF CHECKMATABLE FROM c_cell (PRIMARILY FOR KING USAGE)
        *   @param[in] c_cell checks if this cell can be checkmated(pretends its king)
        *   @pre board is 
        *   @post returns if c_cell can be attacked
        */
        bool isCheckmate(Cell c_cell)
        {
            Cell s_cell= new Cell();

            #region checkPawn
            //checkPawns-----------------
            //check diagonal
            int toMove = (nextToMove==(int)TURN.BLACK) ? 1 :-1; //black is 1,white is -1 
            //right
            if(isSafe(c_cell.r_N+toMove,c_cell.c_N+1) &&
            grid[c_cell.r_N+toMove,c_cell.c_N+1].CurrentlyOccupied && 
            !(sameSide(c_cell.r_N+toMove,c_cell.c_N+1)))
            {
                char name= Char.ToLower(grid[c_cell.r_N+toMove,c_cell.c_N+1].piece.name);
                if(name=='p')
                {                    
                    if(c_cell == KingCell)//do not return, find pieces //todo figure out why I wrote this
                    {
                        KingCheckmatePieces.Add(grid[c_cell.r_N+toMove,c_cell.c_N+1]);
                    }
                    return true;
                }
            }
            
            //left
            if(isSafe(c_cell.r_N+toMove,c_cell.c_N-1) &&
            grid[c_cell.r_N+toMove,c_cell.c_N-1].CurrentlyOccupied && 
            !(sameSide(c_cell.r_N+toMove,c_cell.c_N-1)))
            {
                char name= Char.ToLower(grid[c_cell.r_N+toMove,c_cell.c_N-1].piece.name);
                if(name=='p')
                {                    
                    if(c_cell == KingCell)//do not return, find pieces //todo figure out why I wrote this
                    {
                        KingCheckmatePieces.Add(grid[c_cell.r_N+toMove,c_cell.c_N-1]);
                    }
                    return true;
                }
            }
            //endPawns-----------------
            #endregion
            #region checkKnight
            
            int[] KnM_R = { 2, 2,-2,-2, 1, 1,-1,-1};
            int[] KnM_C = { 1,-1, 1,-1, 2,-2, 2,-2};
            //checkknight
            for(int i_k=0;i_k<8;i_k++)
            {
                if(isSafe(c_cell.r_N+KnM_R[i_k],c_cell.c_N+KnM_C[i_k]) 
                && !(sameSide(c_cell.r_N+KnM_R[i_k],c_cell.c_N+KnM_C[i_k])))
                {
                    char name= Char.ToLower(grid[c_cell.r_N+KnM_R[i_k],c_cell.c_N+KnM_C[i_k]].piece.name);
                    if(name=='n')
                    {                    
                        if(c_cell == KingCell)//do not return, find pieces //todo figure out why I wrote this
                        {
                            KingCheckmatePieces.Add(grid[c_cell.r_N+KnM_R[i_k],c_cell.c_N+KnM_C[i_k]]);
                        }
                        return true;
                    }        
                }
            }
            
            #endregion checkKnight
            #region checkBishop
            //checks if bishop can reach this cell
            int kingpath=4;
            List<string> dummylist= new List<string>();
            
            Sliding_AddMoves(c_cell,ref dummylist,1,int.MaxValue,
            forcheckCritera_positive,true,2,ref kingpath);
            if(kingpath==5)
            {
                return true;
            }
            Sliding_AddMoves(c_cell,ref dummylist,1,int.MaxValue,
            forcheckCritera_positive,true,3,ref kingpath);
            if(kingpath==5)
            {
                return true;
            }        
            Sliding_AddMoves(c_cell,ref dummylist,1,int.MaxValue,
            forcheckCritera_positive,true,4,ref kingpath);
            if(kingpath==5)
            {
                return true;
            }        
            Sliding_AddMoves(c_cell,ref dummylist,1,int.MaxValue,
            forcheckCritera_positive,true,5,ref kingpath);
            if(kingpath==5)
            {
                return true;
            }        
            #endregion
            #region checkRook
            //checks if rook can reach this cell
            //up
            kingpath=3;
            Sliding_AddMoves(c_cell,ref dummylist,c_cell.r_N-1,0,
            forcheckCritera_negative,false,0,ref kingpath);
            if(kingpath==5)
            {
                return true;
            }
            //left
            Sliding_AddMoves(c_cell,ref dummylist,c_cell.c_N-1,0,
            forcheckCritera_negative,false,1,ref kingpath);
            if(kingpath==5)
            {
                return true;
            }
            //down
            Sliding_AddMoves(c_cell,ref dummylist,c_cell.r_N+1,Size,
            forcheckCritera_positive,true,0,ref kingpath);
            if(kingpath==5)
            {
                return true;
            }
            //right
            Sliding_AddMoves(c_cell,ref dummylist,c_cell.c_N+1,Size,
            forcheckCritera_positive,true,1,ref kingpath);
            if(kingpath==5)
            {
                return true;
            }
            #endregion
            #region checkKing
            //checks if a king can reach this square
            
            int[] KM_R = { 0, 0,-1, 1, 1, 1,-1,-1};
            int[] KM_C = { 1,-1, 0, 0, 1,-1, 1,-1};
            for(int i_king=0;i_king<8;i_king++)
            {
                if(isSafe(c_cell.r_N+KM_R[i_king],c_cell.c_N+KM_C[i_king]) 
                && !(sameSide(c_cell.r_N+KM_R[i_king],c_cell.c_N+KM_C[i_king])))
                {
                    char name= Char.ToLower(grid[c_cell.r_N+KM_R[i_king],c_cell.c_N+KM_C[i_king]].piece.name);
                    if(name=='k')
                    {
                        if(c_cell == KingCell)//do not return, find pieces //todo figure out why I wrote this
                        {
                            KingCheckmatePieces.Add(grid[c_cell.r_N+KM_R[i_king],c_cell.c_N+KM_C[i_king]]);
                        }
                        return true;
                    }
                }
            }
            #endregion
            return false;
        }
        private bool checkmateifPieceMoved(Cell c_cell,Cell c_movetoCell)
        {
            bool KingisNowCheckmateifMoved=false;
            Cell[,] save_Grid= new Cell [Size,Size];
            Array.Copy(grid, save_Grid, Size*Size);

            grid[c_movetoCell.r_N,c_movetoCell.c_N]= c_cell;
            grid[c_cell.r_N,c_cell.c_N]= Cell.emptyCell(c_cell.c_N,c_cell.r_N);
            if(c_cell.location==KingCell.location)
            {
                KingisNowCheckmateifMoved = isCheckmate(c_movetoCell);
            }
            else
            {
                KingisNowCheckmateifMoved = isCheckmate(KingCell);
            }
            //reset to orig
            Array.Copy(save_Grid, grid, Size*Size);
            return KingisNowCheckmateifMoved;

        }
        
        /*  Brief: Promotes Piece given string location
        *   @param[in] input_location location to attach dead pieces to revive/promote pawn to
        *   @pre input_location should be at the end of the board
        *   @post attaches possible promotion pieces and returns input_location+(dead pieces of nextToMove)
        */
        public List<string> promotePiece(string input_location)
        {
            List<string> toAdd = new List<string>();
            Dictionary<char,int> sidetoTake = (nextToMove==(int)TURN.BLACK)? deadBlackPieces: deadWhitePieces;
            //Add new promote Moves
            foreach (KeyValuePair<char, int> item in sidetoTake) 
            {
                for(int i=0;i<item.Value;i++)
                {
                    toAdd.Add(input_location+item.Key);
                }
            }
            return toAdd.Distinct().ToList();
        }
        /*  Brief: PawnMoves
        *   @param[in] c_cell cell at which the pawn is located
        *   @pre this should have been called from returnMovesFromCell(Cell) 
        *   @post returns all possible cell location (List<string>) that the pawn can move to
        */
        private List<string> PawnMoves(ref Cell c_cell)
        {
            List<string> toAdd = new List<string>();
            //Move Forward One
            int toMove = (nextToMove==(int)TURN.BLACK) ? 1 :-1; //black is 1,white is -1 
            if(isSafe(c_cell.r_N+toMove,c_cell.c_N))
            {
                //cano
                if(grid[c_cell.r_N+toMove,c_cell.c_N].CurrentlyOccupied==false)
                {
                    if(!checkmateifPieceMoved(c_cell,grid[c_cell.r_N+toMove,c_cell.c_N]))
                    {
                        //promote?
                        if(c_cell.r_N+toMove == 0 || c_cell.r_N+toMove==(Size-1))
                        {
                            toAdd.AddRange(promotePiece(grid[c_cell.r_N+toMove,c_cell.c_N].location));                        
                        }
                        else
                        {
                            toAdd.Add(grid[c_cell.r_N+toMove,c_cell.c_N].location);
                        }
                    }
                }
            }
            
            //Move twice
            if( (c_cell.r_N == 1 && nextToMove==(int)TURN.BLACK) || (c_cell.r_N == 6 && nextToMove==1))
            {
                //previous space isn't occupied                
                if(grid[c_cell.r_N+(toMove),c_cell.c_N].CurrentlyOccupied==false)
                {
                    //space isn't occupied
                    if(grid[c_cell.r_N+(toMove*2),c_cell.c_N].CurrentlyOccupied==false)
                    {
                        
                        if(!checkmateifPieceMoved(c_cell,grid[c_cell.r_N+(toMove*2),c_cell.c_N]))
                        {                        
                            toAdd.Add(grid[c_cell.r_N+(toMove*2),c_cell.c_N].location);
                        }
                    }
                }
            }

            //en passant
            if(enpassant!="-")
            {
                List<int> coord= Cell.convertLocation(enpassant);
                //coord[1] row /coord[0] col
                if(coord[1]==(c_cell.r_N+toMove)) //row
                {
                        //diagonal right
                    if(coord[0]==(c_cell.c_N+1)) //col
                    {
                        if(!checkmateifPieceMoved(c_cell,grid[c_cell.r_N+toMove,c_cell.c_N+1]))
                        {                    
                            toAdd.Add(grid[c_cell.r_N+toMove,c_cell.c_N+1].location);
                        }
                    }   //diagonal left
                    else if(coord[0]==(c_cell.c_N-1))                  
                    {
                        if(!checkmateifPieceMoved(c_cell,grid[c_cell.r_N+toMove,c_cell.c_N-1]))
                        {
                            toAdd.Add(grid[c_cell.r_N+toMove,c_cell.c_N-1].location);
                        }
                    }
                }                           
            }


            //diagonal Capture
            if(isSafe(c_cell.r_N+toMove,c_cell.c_N+1) &&
            grid[c_cell.r_N+toMove,c_cell.c_N+1].CurrentlyOccupied && 
            !(sameSide(c_cell.r_N+toMove,c_cell.c_N+1)))
            {
                if(!checkmateifPieceMoved(c_cell,grid[c_cell.r_N+toMove,c_cell.c_N+1]))
                {
                    if(c_cell.r_N+toMove == 0 || c_cell.r_N+toMove==(Size-1))
                    { //promote move
                        toAdd.AddRange(promotePiece(grid[c_cell.r_N+toMove,c_cell.c_N+1].location));
                    }
                    else
                    {
                        toAdd.Add(grid[c_cell.r_N+toMove,c_cell.c_N+1].location);
                    }
                }
                
            }
            
            if(isSafe(c_cell.r_N+toMove,c_cell.c_N-1) &&
            grid[c_cell.r_N+toMove,c_cell.c_N-1].CurrentlyOccupied && 
            !(sameSide(c_cell.r_N+toMove,c_cell.c_N-1)))
            {
                if(!checkmateifPieceMoved(c_cell,grid[c_cell.r_N+toMove,c_cell.c_N-1]))
                {
                    if(c_cell.r_N+toMove == 0 || c_cell.r_N+toMove==(Size-1))
                    { //promote move
                        toAdd.AddRange(promotePiece(grid[c_cell.r_N+toMove,c_cell.c_N-1].location));

                    }
                    else
                    {
                        toAdd.Add(grid[c_cell.r_N+toMove,c_cell.c_N-1].location);
                    }
                }   
            }
            return toAdd;

        }
        
        /*  Brief: KnightMoves
        *   @param[in] c_cell cell at which the Knight is located
        *   @pre this should have been called from returnMovesFromCell(Cell) 
        *   @post returns all possible cell location (List<string>) that the Knight can move to
        */
        private List<string> KnightMoves(ref Cell c_cell)
        {
            
            int[] KnM_R = { 2, 2,-2,-2, 1, 1,-1,-1};
            int[] KnM_C = { 1,-1, 1,-1, 2,-2, 2,-2};
            List<string> toAdd = new List<string>();
            //Cell s_cell = new Cell();
            for(int i=0;i<8;i++)
            {                
                if(isSafe(c_cell.r_N+KnM_R[i],c_cell.c_N+KnM_C[i]) && !(sameSide(c_cell.r_N+KnM_R[i],c_cell.c_N+KnM_C[i])) 
                && !checkmateifPieceMoved(c_cell,grid[c_cell.r_N+KnM_R[i],c_cell.c_N+KnM_C[i]]))
                {
                    toAdd.Add(grid[c_cell.r_N+KnM_R[i],c_cell.c_N+KnM_C[i]].location);
                }
            }

            
            return toAdd;

        }
        
        private List<string> KingMoves(ref Cell c_cell)
        {
            List<string> toAdd = new List<string>();
          
            
            //NORMAL KING MOVES-------------
            //ROW COLOUMN
            int[] KM_R = { 0, 0,-1, 1, 1, 1,-1,-1};
            int[] KM_C = { 1,-1, 0, 0, 1,-1, 1,-1};
            Cell s_cell= new Cell();
            //
            for(int i=0;i<8;i++)
            {
                if(isSafe(c_cell.r_N+KM_R[i],c_cell.c_N+KM_C[i]) 
                && !(sameSide(c_cell.r_N+KM_R[i],c_cell.c_N+KM_C[i])))
                {
                    s_cell= new Cell (grid[c_cell.r_N+KM_R[i],c_cell.c_N+KM_C[i]]);
                    if(!isCheckmate(s_cell))
                    {
                        toAdd.Add(s_cell.location);
                    }
                }
            }

            //NORMAL KING MOVES-------------


            //Castling //DOUBLE CHECK HERE SINCE TWO PIECES MOVE---------------
            if(nextToMove==(int)TURN.BLACK)//black
            {
                //e8 is king
                if(castling[2]==true) //KingSide
                {
                    //f8,g8
                    //check if there are pieces blocking
                    if(!(grid[0,5].CurrentlyOccupied) && !(grid[0,6].CurrentlyOccupied) && !isCheckmate(KingCell))
                    {
                        //temp move king and rook to check if checkmate when moved

                        //Move rook //put in 5th coloumn
                        //Move move King //put in 6th coloumn
                        grid[0,4] = Cell.emptyCell(4,0);
                        grid[0,5] = Cell.emptyCell(5,0);
                        grid[0,6] = Cell.emptyCell(6,0);
                        grid[0,7] = Cell.emptyCell(7,0);

                        grid[0,5].placePiece('r'); //place rook
                        grid[0,6].placePiece('k'); //place king
                        
                        if(!isCheckmate(grid[0,6]))
                        {
                            toAdd.Add(grid[0,6].location);
                        }
                        
                        grid[0,5] = Cell.emptyCell(5,0);
                        grid[0,6] = Cell.emptyCell(6,0);

                        grid[0,7].placePiece('r'); //place rook
                        grid[0,4].placePiece('k'); //place king
                    }
                }
                if(castling[3]==true) //QueenSide
                {
                    //b8,c8,d8
                    //check if there are pieces blocking
                    if(!(grid[0,1].CurrentlyOccupied) && !(grid[0,2].CurrentlyOccupied) &&
                    !(grid[0,3].CurrentlyOccupied) && !isCheckmate(KingCell))
                    {
                        grid[0,0] = Cell.emptyCell(0,0);

                        grid[0,2] = Cell.emptyCell(2,0);
                        grid[0,3] = Cell.emptyCell(3,0);
                        grid[0,4] = Cell.emptyCell(4,0);

                        grid[0,2].placePiece('r'); //place rook
                        grid[0,4].placePiece('k'); //place king

                        if(!isCheckmate(grid[0,1]))
                        {
                            toAdd.Add(grid[0,2].location);
                        }

                        grid[0,2] = Cell.emptyCell(2,0);
                        grid[0,3] = Cell.emptyCell(3,0);

                        grid[0,0].placePiece('r'); //place rook
                        grid[0,4].placePiece('k'); //place king
                                            
                    }
                }
            }
            else
            {
                //e1 is king
                if(castling[0]==true) //KingSide
                {
                    //f1,g1
                    //check if there are pieces blocking
                    if(!(grid[7,5].CurrentlyOccupied) && !(grid[7,6].CurrentlyOccupied) && !isCheckmate(KingCell))
                    {
                        grid[7,4] = Cell.emptyCell(4,7);
                        grid[7,5] = Cell.emptyCell(5,7);
                        grid[7,6] = Cell.emptyCell(6,7);
                        grid[7,7] = Cell.emptyCell(7,7);

                        grid[7,5].placePiece('R'); //place rook
                        grid[7,6].placePiece('K'); //place king

                        if(!isCheckmate(grid[7,6]))
                        {
                            toAdd.Add(grid[7,6].location);
                        }


                        grid[7,5] = Cell.emptyCell(5,7);
                        grid[7,6] = Cell.emptyCell(6,7);

                        grid[7,7].placePiece('R'); //place rook
                        grid[7,4].placePiece('K'); //place king
                    }
                }
                if(castling[1]==true) //QueenSide
                {
                    //b1,c1,d1
                    //check if there are pieces blocking
                    if(!(grid[7,1].CurrentlyOccupied) && !(grid[7,2].CurrentlyOccupied) &&
                    !(grid[7,3].CurrentlyOccupied) && !isCheckmate(KingCell))
                    {
                        grid[7,0] = Cell.emptyCell(0,7);

                        grid[7,2] = Cell.emptyCell(2,7);
                        grid[7,3] = Cell.emptyCell(3,7);
                        grid[7,4] = Cell.emptyCell(4,7);

                        grid[7,2].placePiece('R'); //place rook
                        grid[7,4].placePiece('K'); //place king

                        if(!isCheckmate(grid[7,1]))
                        {
                            toAdd.Add(grid[7,2].location);
                        }

                        grid[7,2] = Cell.emptyCell(2,7);
                        grid[7,3] = Cell.emptyCell(3,7);

                        grid[7,0].placePiece('R'); //place rook
                        grid[7,4].placePiece('K'); //place king
                    }
                }
            }
            
            return toAdd;                  
        }
        
        /*
        *   @brief centralized function for bishop/rook moves
        *   @param[in] c_cell cell to produce moves from
        *   @param[inout] toAdd list to add the moves to(not used in kingPath>=3)
        *   @param[in] i_start used in forloop i=i_start
        *   @param[in] toIncrease the way the for loop should increase (1/-1)(T/F)
        *   @param[in] i_end used in forloop  tocheck when to end
        *   @param[in] mode 0-5 checks which mode use for left() and right() 
        *   @param[inout] kingpath 
        *   kingpath: 0 not finding path to king, 1-2 find path to king, if ==2 king has been found
        *   3-5 iskingcheckmate(3 is rookmoves 4 is bishopmoves), if ==5 king has been checkmated
        *   @pre i_stopcriteria must have coincide with i_start and i_end and vice versa
        *   @pre appropriate values for i_start,i_end,toIncrease,mode,kingPath for the function to work
        *   @post when kingpath<2, returns moves that can be produced from the cell
        *   @post when kingpath==2, returns if king has been found
        *   @post when kingpath>=3, returns if king is checkmated by changing kingpath to 5
        */
        private void Sliding_AddMoves( Cell c_cell,ref List<string> toAdd,
        int i_start,int i_end,Func<int, int, bool> i_stopcriteria,bool toIncrease,int mode,ref int kingPath)
        {
            Func<int> left; //row variable
            Func<int> right; //right varialble
            Cell s_cell= new Cell();
            int increaseby=(toIncrease)? 1 : -1;
            int i=0;
            
            if(mode==0) //Check
            {
                left= ()=>i;
                right=()=>c_cell.c_N;
            }
            else if(mode==1)
            {
                left= ()=>c_cell.r_N;
                right=()=>i;
            }
            else if(mode==2) //Check
            {
                //up-left
                left= ()=>c_cell.r_N-i;
                right=()=>c_cell.c_N-i;
            }
            else if(mode==3)
            {
                //down-right
                left= ()=>c_cell.r_N+i;
                right=()=>c_cell.c_N+i;
            }
            else if(mode==4)
            {
                //down-left
                left= ()=>c_cell.r_N+i;
                right=()=>c_cell.c_N-i;
            }
            else if(mode==5)
            {
                //up-right
                left= ()=>c_cell.r_N-i;
                right=()=>c_cell.c_N+i;
            }
            else
            {
                throw new System.ArgumentException("Parameter is invalid ", "mode: "+mode);
            }

            for(i = i_start;i_stopcriteria(i,i_end);i+=increaseby)
            {
                if(isSafe(left(),right()))
                {
                    s_cell= new Cell (grid[left(),right()]);
                    if(kingPath==0) //used to generate moves from rook/bishop/queen
                    {
                        
                        if(sameSide(left(),right()))
                        {
                            break;
                        }
                        if(!checkmateifPieceMoved(c_cell,s_cell))
                        {
                            toAdd.Add(s_cell.location);
                        }
                    
                        if(grid[left(),right()].CurrentlyOccupied) //no more moves to place
                        {
                            break;
                        }
                    }
                    else if(kingPath==1) //used in finding pathtofindking
                    {
                        if(s_cell.location == KingCell.location)
                        {
                            kingPath=2; //stop, king has been found
                            break;
                        }
                        else if(s_cell.isEmpty())
                        {
                            toAdd.Add(s_cell.location);
                        }
                    
                        if(grid[left(),right()].CurrentlyOccupied) //no more moves to place
                        {
                            break;
                        }
                    }
                    else if(kingPath>=3) //used in is checkmate
                    {
                        if(sameSide(left(),right()) && s_cell.location!=KingCell.location)
                        {
                            break;
                        }
                        if(s_cell.location=="b2" ||c_cell.location=="f6")
                        {

                        }
                        if(s_cell.CurrentlyOccupied && s_cell.location!=KingCell.location) //no more moves to place
                        {
                            char name= Char.ToLower(s_cell.piece.name);
                            if( (kingPath==3 && name=='r') || (kingPath==4 && name=='b') || name=='q')
                            {
                                if(c_cell.location == KingCell.location)//do not return, find pieces //todo figure out why I wrote this
                                {
                                    KingCheckmatePieces.Add(s_cell);
                                }
                                kingPath=5;
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                    else
                    {
                        throw new System.ArgumentException("Parameter is invalid ", "kingpath: "+kingPath);
                    }
                }
                else
                {
                    break;
                }
            }
        }
        
        /*  Brief: RookMoves
        *   @param[in] c_cell cell at which the Rook is located
        *   @pre this should have been called from returnMovesFromCell(Cell) 
        *   @post returns all possible cell location (List<string>) that the Rook can move to
        */                
        private List<string> RookMoves(ref Cell c_cell)
        {
            Cell s_cell= new Cell();
            List<string> toAdd = new List<string>();
            int dummy=0;
            //up
            Sliding_AddMoves(c_cell,ref toAdd,c_cell.r_N-1,0,
            forcheckCritera_negative,false,0,ref dummy);
            //left
            Sliding_AddMoves(c_cell,ref toAdd,c_cell.c_N-1,0,
            forcheckCritera_negative,false,1,ref dummy);
            //down
            Sliding_AddMoves(c_cell,ref toAdd,c_cell.r_N+1,Size,
            forcheckCritera_positive,true,0,ref dummy);
            //right
            Sliding_AddMoves(c_cell,ref toAdd,c_cell.c_N+1,Size,
            forcheckCritera_positive,true,1,ref dummy);
            return toAdd;                        
        }
        /*  Brief: BishopMoves
        *   @param[in] c_cell cell at which the Bishop is located
        *   @pre this should have been called from returnMovesFromCell(Cell) 
        *   @post returns all possible cell location (List<string>) that the Bishop can move to
        */

        private List<string> BishopMoves(ref Cell c_cell)
        {
            List<string> toAdd = new List<string>();
            int dummy=0;
            //up-left
            //down-right
            //down-left
            //up-right
            Sliding_AddMoves(c_cell,ref toAdd,1,int.MaxValue,
            forcheckCritera_positive,true,2,ref dummy);            
            Sliding_AddMoves(c_cell,ref toAdd,1,int.MaxValue,
            forcheckCritera_positive,true,3,ref dummy);
            Sliding_AddMoves(c_cell,ref toAdd,1,int.MaxValue,
            forcheckCritera_positive,true,4,ref dummy);
            Sliding_AddMoves(c_cell,ref toAdd,1,int.MaxValue,
            forcheckCritera_positive,true,5,ref dummy);
            return toAdd;

        }
        
        /*  Brief: findpathtoKingRook
        *   @param[in] c_cell cell at which the threatening Rook is located
        *   @pre this should have been called when current king is checkmated
        *   @post returns all possible cell location (List<string>) to nullify Rook threat
        */

        private List<string> findpathtoKingRook(Cell c_cell)
        {
            List<string> toAdd = new List<string>();
            int kingpath=1;
            //up
            Sliding_AddMoves(c_cell,ref toAdd,c_cell.r_N-1,0,
            forcheckCritera_negative,false,0,ref kingpath);
            if(kingpath==2)
            {
                return toAdd;
            }
            toAdd.Clear();
            //left
            Sliding_AddMoves(c_cell,ref toAdd,c_cell.c_N-1,0,
            forcheckCritera_negative,false,1,ref kingpath);
            if(kingpath==2)
            {
                return toAdd;
            }
            toAdd.Clear();
            //down
            Sliding_AddMoves(c_cell,ref toAdd,c_cell.r_N+1,Size,
            forcheckCritera_positive,true,0,ref kingpath);
            if(kingpath==2)
            {
                return toAdd;
            }
            toAdd.Clear();
            //right
            Sliding_AddMoves(c_cell,ref toAdd,c_cell.c_N+1,Size,
            forcheckCritera_positive,true,1,ref kingpath);
            if(kingpath==2)
            {
                return toAdd;
            }
            toAdd.Clear();
            
            return toAdd;                        
        }

        /*  Brief: findpathtoKingBishop
        *   @param[in] c_cell cell at which the threatening Bishop is located
        *   @pre this should have been called when current king is checkmated
        *   @post returns all possible cell location (List<string>) to nullify Bishop threat
        */

        private List<string> findpathtoKingBishop(Cell c_cell)
        {
            List<string> toAdd = new List<string>();
            int kingpath=1;
            
            Sliding_AddMoves(c_cell,ref toAdd,1,int.MaxValue,
            forcheckCritera_positive,true,2,ref kingpath);
            if(kingpath==2)
            {
                return toAdd;
            }
            toAdd.Clear();
            Sliding_AddMoves(c_cell,ref toAdd,1,int.MaxValue,
            forcheckCritera_positive,true,3,ref kingpath);
            if(kingpath==2)
            {
                return toAdd;
            }        
            toAdd.Clear();
            Sliding_AddMoves(c_cell,ref toAdd,1,int.MaxValue,
            forcheckCritera_positive,true,4,ref kingpath);
            if(kingpath==2)
            {
                return toAdd;
            }        
            toAdd.Clear();
            Sliding_AddMoves(c_cell,ref toAdd,1,int.MaxValue,
            forcheckCritera_positive,true,5,ref kingpath);
            if(kingpath==2)
            {
                return toAdd;
            }        
            toAdd.Clear();
            return toAdd;

        }
        /*  Brief: updateBoard
        *   @param[in] FEN string to update the board to (clears out board first)
        *   @pre thisBoard should be 8x8 standard chess
        *   @post updates the state of the board to the fenstring notation
        */
        public void updateBoard(string FEN)
        {
            //clearBoard
            
            for (int y =0; y<Size;y++)
            {
                for (int x =0; x<Size;x++)
                {
                    grid[y,x].clear();
                }
            }
            WhitePieces.Clear();
            BlackPieces.Clear();
            //cellsopponentCanMoveTo.Clear();
            resetDeadPieces();
            KingCell= new Cell();
            opponentKingCell= new Cell();
            BlackScore=0;
            WhiteScore=0;
            moveHistory.Clear();

            //Interpret FEN
            List<string> splitFEN= new List<string>(FEN.Split(' '));
            List<string> BoardState=new List<string>(splitFEN[0].Split('/'));

            //next to move
            string temp=splitFEN[1];
            if(temp=="b") //black
            {
                nextToMove=(int)TURN.BLACK;
            }
            else
            {
                nextToMove=(int)TURN.WHITE;
            }


            //process castling
            string tempcastling=splitFEN[2];
            for(int i=0;i<4;i++)
            {
                castling[i]=false;
            }
            if(tempcastling!="-")
            {
                char[] charArr= tempcastling.ToCharArray();
                foreach(char c in charArr)
                {
                    if(c=='K') //Skip Lines
                    {
                        castling[0]=true;
                    }
                    else if(c=='Q')
                    {
                        castling[1]=true;
                    }
                    else if(c=='k') //black
                    {
                        castling[2]=true;                        
                    }
                    else if(c=='q') //black
                    {
                        castling[3]=true;                        
                    }
                }
            } 
                       
            

            enpassant=splitFEN[3];
            halfmove=int.Parse(splitFEN[4]);
            fullmove=int.Parse(splitFEN[5]);
            
            //Place Pieces
            int row=0;
            foreach(string line in BoardState)
            {
                char[] charArr= line.ToCharArray();
                int coloum=0;
                foreach(char c in charArr)
                {   
                    if(Char.IsDigit(c)) //Skip Lines
                    {
                        int numtoPass=int.Parse(c.ToString());
                        coloum+=numtoPass;                                      
                    }
                    else    //Place Pieces
                    {                   
                        bool isWhite=grid[row,coloum].placePiece(c);
                        if(isWhite)
                        {
                            WhitePieces.Add(grid[row,coloum]);
                            WhiteScore+=grid[row,coloum].piece.value; //add value
                            deadWhitePieces[c] = deadWhitePieces[c]-1;
                            if(c=='K' )
                            {
                                if(nextToMove==(int)TURN.WHITE)
                                {
                                    KingCell=grid[row,coloum];
                                }
                                else
                                {
                                    opponentKingCell=grid[row,coloum];
                                }
                            }
                        }
                        else
                        {
                            BlackPieces.Add(grid[row,coloum]);
                            BlackScore+=grid[row,coloum].piece.value; //add value
                            deadBlackPieces[c] = deadBlackPieces[c]-1;
                            if(c=='k')
                            {
                                if(nextToMove==(int)TURN.BLACK)
                                {
                                    KingCell=grid[row,coloum];
                                }
                                else
                                {
                                    opponentKingCell=grid[row,coloum];
                                }
                            }
                        }
                        coloum++;                           
                    }
                }
                row++;
            }

        }
        
        /*  Brief: isSafe
        *   @param[in] y row to check at
        *   @param[in] x coloumn to check at
        *   @post returns if x,y is within bounds
        */
        private bool isSafe(int y,int x)
        {
            if( y<0 || x<0 || y>=Size || x>=Size) //out of bounds
            {
                return false;
                
            }
            return true;
        }
        /*  Brief: sameSide
        *   @param[in] y row to check at
        *   @param[in] x coloumn to check at
        *   @post returns if x,y cell location is the same side as the nextToMove player (true)
        */
        private bool sameSide(int y, int x)
        {
            //same side
            if(grid[y,x].CurrentlyOccupied)
            {
                if(grid[y,x].piece.colour == nextToMove)
                    return true;
            }
            return false;
        }
        /*  Brief: ToString()
        *   @post returns state of board and deadPieces()
        */
        
        public override string ToString()
        {
           return CurrentBoard()+deadPieces()+"\n--END---\n";
        }
        /*  Brief: DebugBoard()
        *   @post returns locations of the board (used for debugging)
        */
        public string DebugBoard()
        {
            string BoardRepresentation="";
            for (int y =0; y<Size;y++)
            {
                for (int x =0; x<Size;x++)
                {
                    BoardRepresentation+="["+grid[y,x].location+"]";
                }
                BoardRepresentation+="\n";
            }
            BoardRepresentation+="\n";
            return BoardRepresentation;

        }
        /*  Brief: CurrentBoard()
        *   @post returns current state of board
        */

        private string CurrentBoard()
        {
            string BoardRepresentation="";
            for (int y =0; y<Size;y++)
            {
                for (int x =0; x<Size;x++)
                {
                    BoardRepresentation+="[";
                    Cell c = new Cell(grid[y,x]);
                    if(c.CurrentlyOccupied)
                    {
                        BoardRepresentation+=grid[y,x].piece.ToString();
                    }
                    else
                    {
                        BoardRepresentation+=".";      

                    }
                    BoardRepresentation+="]";
                }
                BoardRepresentation+="\n";
            }
            BoardRepresentation+="\n";
            return BoardRepresentation;
        }
        /*  Brief: OccupiedList()
        *   @post returns occupied states of board
        */
        private string OccupiedList()
        {
            string BoardRepresentation="";
            for (int y =0; y<Size;y++)
            {
                for (int x =0; x<Size;x++)
                {
                    BoardRepresentation+="[";
                    Cell c = new Cell(grid[y,x]);
                    if(c.CurrentlyOccupied)
                    {
                        BoardRepresentation+=grid[y,x].CurrentlyOccupied;
                    }
                    else
                    {
                        BoardRepresentation+=".";      

                    }
                    BoardRepresentation+="]";
                }
                BoardRepresentation+="\n";
            }
            BoardRepresentation+="\n";
            return BoardRepresentation;
        }

        
        /*  Brief: deadPieces()
        *   @post returns deadpieces state
        */
        public string deadPieces()
        {
            string dead_ps = "";
            dead_ps += "-----------------\n";            
            dead_ps += "BlackPieces\n";
            
            foreach (KeyValuePair<char, int> item in deadBlackPieces) 
            {
                dead_ps+= item.Key+":"+ item.Value+"\n";
            }
            dead_ps += "\nWhitePieces\n";
            foreach (KeyValuePair<char, int> item in deadWhitePieces) 
            {
                dead_ps+= item.Key+":"+ item.Value+"\n";
            }
            dead_ps += "-----------------\n";
            return dead_ps;
        }


    }
}
