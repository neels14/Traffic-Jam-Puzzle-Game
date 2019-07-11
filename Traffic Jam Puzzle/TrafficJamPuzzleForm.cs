
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TrafficJamPuzzle
{

    public partial class TrafficJamPuzzleForm : Form
    {
        #region Global Structs and Fields
        /*
         * A struct type is a value type that is typically used to encapsulate small groups of related variables.
         * The struct type is suitable for representing lightweight objects. While a class could be used to do much
         * the same thing, structs are often more efficient.
         * For more information, see https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/struct
         * and https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/using-structs.
         */

        //Create a PuzzleSolutionArray
        //SolveFromLeft
        /*int[,] puzzleSolutionArrayLeft = new int[35, 2] {{4,5}, {6,4}, {7,6}, {5,7}, {3,5}, {2,3}, {4,2}, {6,4}, {8,6}, {9,8}, {7,9},
                                           {5,7}, {3,5}, {1,3}, {0,1}, {2,0}, {4,2}, {6,4}, {8,6}, {10,8}, {9,10}, {7,9},
                                           {5,7}, {3,5}, {1,3}, {2,1}, {4,2}, {6,4}, {8,6}, {7,8}, {5,7}, {3,5}, {4,3}, {6,4}, {5,6}};
        */
        /*Solution Moves(LEFT)
         * (moveFrom,moveTo)-1
         * (5,6)(7,5)(8,7)(6,8)(4,6)(3,4)(5,3)(7,5)(9,7)(10,9)(8,10)
         * (6,8)(4,6)(2,4)(1,2)(3,1)(5,3)(7,5)(9,7)(11,9)(10,11)(8,10)
         * (6,8)(4,6)(2,4)(3,2)(5,3)(7,5)(9,7)(8,9)(6,8)(4,6)(5,4)(7,5)(6,7)
         */

        //SolveFromRight
        /*int[,] puzzleSolutionArrayRight = new int[35, 2] { { 6, 5 }, { 4, 6 }, { 3, 4 }, { 5, 3 }, { 7, 5 }, { 8, 7 }, { 6, 8 }, { 4, 6 },
                                                 { 2, 4 }, { 1, 2 }, { 3, 1 }, { 5, 3 }, { 7, 5 }, { 9, 7 }, { 10, 9 }, { 8, 10 }, { 6, 8 },
                                                 { 4, 6 }, { 2, 4 }, { 0, 2 }, { 1, 0 }, { 3, 1 }, { 5, 3 }, { 7, 5 }, { 9, 7 }, { 8, 9 },
                                                 { 6, 8 }, { 4, 6 }, { 2, 4 }, { 3, 2 }, { 5, 3 }, { 7, 5 }, { 6, 7 }, { 4, 6 }, { 5, 4 } };
        */
        /*Solution Moves(RIGHT)
         * (moveFrom,moveTo)-1
         * { 7, 6 }, { 5, 7 }, { 4, 5 }, { 6, 4 }, { 8, 6 }, { 9, 8 }, { 7, 9 }, { 5, 7 },
         * { 3, 5 }, { 2, 3 }, { 4, 2 }, { 6, 4 }, { 8, 6 }, { 10, 8 }, { 11, 10 }, { 9, 11 }, { 7, 9 },
         * { 5, 7 }, { 3, 5 }, { 1, 3 }, { 2, 1 }, { 4, 2 }, { 6, 4 }, { 8, 6 }, { 10, 8 }, { 9, 10 },
         * { 7, 9 }, { 5, 7 }, { 3, 5 }, { 4, 3 }, { 6, 4 }, { 8, 6 }, { 7, 8 }, { 5, 7 }, { 6, 5 } 
         */


        int[] SolutionArrayLeft = new int[35] { 4, 6, 7, 5, 3, 2, 4, 6, 8, 9, 7, 5, 3, 1, 0, 2, 4, 6, 8, 10, 9, 7, 5, 3, 1, 2, 4, 6, 8, 7, 5, 3, 4, 6, 5 };
        int[] SolutionArrayRight = new int[35] { 6, 4, 3, 5, 7, 8, 6, 4, 2, 1, 3, 5, 7, 9, 10, 8, 6, 4, 2, 0, 1, 3, 5, 7, 9, 8, 6, 4, 2, 3, 5, 7, 6, 4, 5 };


        // The following struct is used to encapsulate the data needed to describe a legal move.
        private struct MoveData
        {
            public int movedFromIndex;
            public int movedToIndex;

            // Constructor. Used to initialize the above variables.
            public MoveData(int movedFrom, int movedTo)
            {
                movedFromIndex = movedFrom;
                movedToIndex = movedTo;
            }
        }

        private int moves = 0;

        //Used to keep track of the source and target picture boxes for drag and drop, as well as direction of move.
        private MoveData currentMove = new MoveData(0, 0);

        //Used to undo moves. A stack is a "last-in-first-out" collection (LIFO).
        //Items are always added to and removed from the top of a stack.
        //Adding to a stack is called "pushing" and removing from a stack is called "popping."
        private Stack<MoveData> moveStack = new Stack<MoveData>();

        //Declare the array "position" WITHOUT instantiating it. 
        //This array is used to keep track of the positions of the people crossing the river.
        private PictureBox[] position;


        //The pictures are stored in the resource file for the project.
        private static Image walkingRightImage = Properties.Resources.WalkingRight;
        private static Image walkingLeftImage = Properties.Resources.WalkingLeft;

        #endregion

        #region Constructor Method
        /*
         * Constructor method for the "TrafficJamPuzzleForm" class
         * 
         * This method MUST contain a call to the "InitializeComponent" method.
         * It should also contain any initialization code (i.e. code that needs 
         * to be executed BEFORE the user interacts with the user interface).
         */
        public TrafficJamPuzzleForm()
        {
            //Initialize components created using the Form Designer
            InitializeComponent();

            //Display description and rules of the game

            descriptionLabel.Text = "Two groups of 5 people are crossing a river in opposite directions by hopping on stones. There are eleven stones\n" +
                "(numbered 1 − 11) and in the starting configuration, the empty stone is between the two groups. The objective is\n" +
                "for the two groups to exchange sides, that is, to pass each other.\n\n";

            rulesLabel.Text = "Make moves by dragging and dropping.\n" +
                "1. You can only move forward in the direction you are facing. No backstepping or turning around is allowed.\n" +
                "2. Only one person may move at a time.\n" +
                "3. There are only two types of legal moves:\n" +
                "    (a) You may move into an adjacent empty space immediately in front of you.\n" +
                "    (b) You may move around a person (or leapfrog over them) into an empty space behind them,\n" +
                "         if they are adjacent to and facing you.";

            //Instantiate and initialize the array "position."
            //Storing the picture boxes in an array makes it possible to use ONE event handler for ALL the picture boxes.
            position = new PictureBox[11] {pictureBox1,pictureBox2, pictureBox3,pictureBox4, pictureBox5,
                                           pictureBox6, pictureBox7, pictureBox8,pictureBox9,pictureBox10,
                                           pictureBox11 };

            for (int i = 0; i < position.Length; i++)
            {
                //Do not allow drops on occupied positions
                position[i].AllowDrop = false;

                //Load pictures in the picture boxes. 
                if (i >= 0 && i <= 4)
                {
                    position[i].Image = walkingRightImage;
                }
                else if (i >= 6)
                {
                    position[i].Image = walkingLeftImage;
                }

                //Set event handlers for each picture box.
                position[i].MouseDown += PositionPictureBox_MouseDown;
                position[i].DragEnter += PositionPictureBox_DragEnter;
                position[i].DragDrop += PositionPictureBox_DragDrop;
            }

            // Allow drops ONLY on the ONLY unoccuopied position
            position[5].AllowDrop = true;

        }
        #endregion

        #region Event Handlers

        // The "MouseDown" event must be handled to detect the initiation of a drag
        private void PositionPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            // Determine the index of the picture box on which the drag was initiated.
            currentMove.movedFromIndex = IndexOfObject(sender, position);

            if (currentMove.movedFromIndex >= 0)
            {
                Image img = position[currentMove.movedFromIndex].Image;

                //If non-blank picture box is being dragged and move is in progress
                if (img != null && DoDragDrop(img, DragDropEffects.Move) == DragDropEffects.Move) 
                {
                    moveStack.Push(new MoveData(currentMove.movedFromIndex, currentMove.movedToIndex));

                    position[currentMove.movedFromIndex].Image = null; //Remove image

                    //Stalement Detection Code Here
                    if(IsStalemate(currentMove.movedFromIndex))
                    {
                        MessageBox.Show("A Stalemate has been detected, there are no more moves that could be made to solve the puzzle! Please undo or restart the game!", "STALEMATE DETECTION");
                    }
                    

                    position[currentMove.movedFromIndex].AllowDrop = true; //Allow drop now that stone is unoccupied
                    position[currentMove.movedToIndex].AllowDrop = false; //Don't allow drop now that stone is occupied

                    moves++;
                    int undoneMoves = moves - moveStack.Count;

                    //Display information on number of moves made
                    movesLabel.Text = moves.ToString();
                    undoneMovesLabel.Text = undoneMoves.ToString();
                    netMovesLabel.Text = (moves - undoneMoves).ToString();

                    if (IsPuzzleSolved(position))
                    {
                        MessageBox.Show("You solved the puzzle in " + moves.ToString() + " moves, which includes " + undoneMoves.ToString() + " undone moves.", "Aren't you clever?");
                    }

                }

            }
        }

        // A "DragEnter" event occurs when an object is dragged into a control's bounds.
        // e.g. A "DragEnter" event occurs when an image is dragged into a picture box.

        private void PositionPictureBox_DragEnter(object sender, DragEventArgs e)
        {
            // Determine the index of the picture box into which the object ("sender") is being dragged.
            currentMove.movedToIndex = IndexOfObject(sender, position);

            if (currentMove.movedToIndex >= 0)
            {
                //If Correct type of data is being dragged (image in this case) and drop is occurring on an empty position
                if (e.Data.GetDataPresent(DataFormats.Bitmap) && position[currentMove.movedToIndex].AllowDrop == true)
                {
                    if (IsLegalMove(position, currentMove.movedFromIndex, currentMove.movedToIndex))
                    {
                        e.Effect = DragDropEffects.Move; //Move is in progress
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None; //Move is not in progress
                    }
                }
            }
            
        }

        private void PositionPictureBox_DragDrop(object sender, DragEventArgs e)
        {
            if (currentMove.movedToIndex >= 0)
            {
                position[currentMove.movedToIndex].Image = (Bitmap)e.Data.GetData(DataFormats.Bitmap);
            }
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            if (moveStack.Count > 0)
            {
                MoveData lastMove = moveStack.Pop();

                position[lastMove.movedFromIndex].Image = position[lastMove.movedToIndex].Image;
                position[lastMove.movedFromIndex].AllowDrop = false;
                position[lastMove.movedToIndex].Image = null;
                position[lastMove.movedToIndex].AllowDrop = true;

                int undoneMoves = moves - moveStack.Count;
                undoneMovesLabel.Text = undoneMoves.ToString();
                netMovesLabel.Text = (moves - undoneMoves).ToString();
            }
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void UndoButton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                UndoButton_Click(sender, e);
            }
        }

        private void UndoFocus_MouseUp(object sender, MouseEventArgs e)
        {
            UndoButton.Focus();
        }
        
        private void TrafficJamPuzzleForm_MouseEnter(object sender, EventArgs e)
        {
            Cursor.Clip = this.Bounds;
        }

        private void ShowSolution_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To solve the puzzle, you need to make sure you never have two of the same facing people side by side.\nThe moves are as follows with the format (person's current position, move to this position):\nFrom the Left:\n(5,6)(7,5)(8,7)(6,8)(4,6)(3,4)(5,3)(7,5)(9,7)(10,9)(8,10)\n(6,8)(4,6)(2,4)(1,2)(3,1)(5,3)(7,5)(9,7)(11,9)(10,11)(8,10)\n(6,8)(4,6)(2,4)(3,2)(5,3)(7,5)(9,7)(8,9)(6,8)(4,6)(5,4)(7,5)(6,7)\nFrom the right:\n(7, 6)(5, 7)(4, 5)(6, 4)(8, 6)(9, 8)(7, 9)(5, 7)\n(3, 5)(2, 3)(4, 2)(6, 4)(8, 6)(10, 8)(11, 10)(9, 11)(7, 9)\n(5, 7)(3, 5)(1, 3)(2, 1)(4, 2)(6, 4)(8, 6)(10, 8)(9, 10)\n(7, 9)(5, 7)(3, 5 )(4, 3)(6, 4)(8, 6)(7, 8)(5, 7)(6, 5)","SOLUTION");
        }

        #endregion

        #region Other Methods

        private static bool IsLegalMove(PictureBox[] position, int fromIndex, int toIndex)
        {
            //The "AllowDrop" property is set to "true" only for the sole empty picture box. This makes 
            //it impossible to drop an image on any picture box that contains a picture. Therefore, it's
            //only necessary to check whether the move does not violate the rules of the game. It's not
            //necessary to check whether a drop occurs on an empty picture box.

            int directionFacing = (position[fromIndex].Image == walkingRightImage) ? 1 : -1;
            int changeInPosition = toIndex - fromIndex;

            if (changeInPosition * directionFacing == 1) //Moving one spot in same direction person is facing
            {
                return true;
            }
            else if (changeInPosition * directionFacing == 2)//Moving two spots in same direction person is facing
            {
                //Check whether person being "leapfrogged" is facing in the opposite direction. Note that the following
                //code could be written without creating the temporary variable "interveningPersonDirectionFacing."
                //However, this would seriously compromise the understandability of the code.
                int interveningPersonDirectionFacing = (position[fromIndex + changeInPosition / 2].Image == walkingRightImage) ? 1 : -1;
                if (interveningPersonDirectionFacing != directionFacing)
                {
                    return true;
                }
            }

            return false; //Move is invalid. Only reached if the conditions in BOTH of above "if" statement clauses are false.
        }

        private static bool IsPuzzleSolved(PictureBox[] position)
        {
            //Are people in first 5 positions all facing to the left?
            for (int i = 0; i < 5; i++)
            {
                int directionFacing = (position[i].Image == walkingRightImage) ? 1 : -1;
                if (directionFacing != -1 || position[i].Image == null)
                {
                    return false;
                }
            }

            //Are people in last 5 positions all facing to the right?
            for (int i = 6; i < position.Length; i++)
            {
                int directionFacing = (position[i].Image == walkingRightImage) ? 1 : -1;
                if (directionFacing != 1 || position[i].Image == null)
                {
                    return false;
                }
            }

            return true;
        }

        private static int IndexOfObject(object o, object[] a)
        {
            if (a.Length > 0)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (o.Equals(a[i]))
                    {
                        return i;
                    }
                }
                return -1; //Not found
            }
            return -1; //Array is empty
        }

        //Stalemate Method
        private bool IsStalemate(int MovedFrom)
        {
            int moveNumber = moves - (moves - moveStack.Count);
            Console.WriteLine(moveNumber);
            bool stalemateDetected = true;
            
            if (SolutionArrayLeft[moveNumber - 1] == MovedFrom || SolutionArrayRight[moveNumber - 1] == MovedFrom)
            {
                stalemateDetected = false;
            }
            return stalemateDetected;
        }
        

        #endregion

    }
}
