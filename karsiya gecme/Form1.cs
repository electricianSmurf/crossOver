using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace crossOver
{
    public partial class Form1 : Form
    {
        Random rnd = new Random();

        List<PictureBox> lstBalls = new List<PictureBox>();

        List<int> lstBallsStartLocX = new List<int>();
        List<int> lstBallsStartLocY = new List<int>();

        List<int> lstFirstGroupFromLeft = new List<int>();
        List<int> lstFirstGroupFromRight = new List<int>();

        List<int> lstSecondGroupFromLeft = new List<int>();
        List<int> lstSecondGroupFromRight = new List<int>();

        List<int> lstThirdGroupFromLeft = new List<int>();
        List<int> lstThirdGroupFromRight = new List<int>();

        List<bool> lstIsBallMoving = new List<bool>();

        bool isBallOnLeftOrRight;
        bool isLocationFrontOrBack = true;
        
        bool firstGroupMoving = true;
        bool firstGroupAtMiddle;
        bool firstGroupAtEnd;

        bool secondGroupMoving;
        bool secondGroupAtMiddle;
        bool secondGroupAtEnd;

        bool thirdGroupMoving;
        bool thirdGroupAtMiddle;
        bool thirdGroupAtEnd;

        bool isPushedUp;
        bool isPushedDown;

        int ballVelocity = 8;

        int health = 5;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            lblHealth.Text = "Health: " + health.ToString();

            calculateStartLocY();

            createBalls();

            detectFirstGroupBalls();
            detectSecondGroupBalls();
            detectThirdGroupBalls();
        }
        void calculateStartLocY()
        {
            for (int i = 1; i < 37; i++)
            {
                lstBallsStartLocY.Add(13 * i);
                lstIsBallMoving.Add(false);
            }
        }
        void createBalls()
        {
            createLeftGroupBalls();

            isBallOnLeftOrRight = !isBallOnLeftOrRight;

            createRightGroupBalls();
        }
        void createLeftGroupBalls()
        {
            PictureBox pb;
            for (int i = 1; i < lstBallsStartLocY.Count; i += 2)
            {
                pb = createPBoxObject();
                pb.Location = new Point(detectBallLocationX(), lstBallsStartLocY[i]);
                lstBalls.Add(pb);
                Controls.Add(pb);
            }
        }
        private PictureBox createPBoxObject()
        {
            PictureBox pbox = new PictureBox();
            pbox.BackColor = Color.Transparent;
            pbox.Image = Properties.Resources.ball;
            pbox.SizeMode = PictureBoxSizeMode.StretchImage;
            pbox.Size = new Size(10, 10);
            return pbox;
        }
        private int detectBallLocationX()
        {
            int locX = 0;
            if (!isBallOnLeftOrRight)
            {
                isLocationFrontOrBack = !isLocationFrontOrBack;
                locX = -12;
                if (!isLocationFrontOrBack)
                {
                    locX = -48;
                }
            }
            else //(isBallOnLeftOrRight)
            {
                isLocationFrontOrBack = !isLocationFrontOrBack;
                locX = 412;
                if (!isLocationFrontOrBack)
                {
                    locX = 448;
                }
            }
            lstBallsStartLocX.Add(locX);
            return locX;
        }
        void createRightGroupBalls()
        {
            PictureBox pb;
            for (int i = 0; i < lstBallsStartLocY.Count; i += 2)
            {
                pb = createPBoxObject();
                pb.Location = new Point(detectBallLocationX(), lstBallsStartLocY[i]);
                lstBalls.Add(pb);
                Controls.Add(pb);
            }
        }
        private int detectRandomBall(bool isLeftOrRight)//false: left group, true: right group
        {
            int number = rnd.Next(0, 18);
            if (isLeftOrRight)
            {
                number = rnd.Next(18, 36);
            }
            return number;
        }
        void detectFirstGroupBalls()
        {
            fillBallGroupList(lstFirstGroupFromLeft, false);

            fillBallGroupList(lstFirstGroupFromRight, true);
        }
        void detectSecondGroupBalls()
        {
            fillBallGroupList(lstSecondGroupFromLeft, false);

            fillBallGroupList(lstSecondGroupFromRight, true);
        }
        void detectThirdGroupBalls()
        {
            fillBallGroupList(lstThirdGroupFromLeft, false);

            fillBallGroupList(lstThirdGroupFromRight, true);
        }
        void fillBallGroupList(List<int> recievedList, bool isLeftOrRight)//false: left group, true: right group
        {
            int ballNumber;
            for (int i = 0; i < 6; i++)
            {
                if (recievedList.Count < 6)
                {
                    ballNumber = detectRandomBall(isLeftOrRight);
                    if (!lstIsBallMoving[ballNumber])
                    {
                        recievedList.Add(ballNumber);
                        lstIsBallMoving[ballNumber] = true;
                    }
                    else
                        i--;
                }
            }
        }
        private void ballMover_Tick(object sender, EventArgs e)
        {
            checkCrushDetection();
            if (health == 0 || health == 10)
            {
                gameOver();
            }
            if (!firstGroupMoving && !secondGroupMoving && thirdGroupAtMiddle)
            {
                detectFirstGroupBalls();
                detectSecondGroupBalls();

                thirdGroupAtMiddle = false;

                firstGroupMoving = true;
            }

            if (firstGroupMoving)
            {
                firstGroupMover();
            }

            if (firstGroupAtMiddle)
            {
                firstGroupAtMiddle = false;

                secondGroupMoving = true;
            }

            if (secondGroupMoving)
            {
                secondGroupMover();
            }
            if (secondGroupAtMiddle)
            {
                detectThirdGroupBalls();

                secondGroupAtMiddle = false;

                thirdGroupMoving = true;
            }

            if (thirdGroupMoving)
            {
                thirdGroupMover();
            }
        }
        void checkCrushDetection()
        {
            for (int i = 0; i < lstBalls.Count; i++)
            {
                if (lstBalls[i].Bounds.IntersectsWith(plane_pb.Bounds))
                {
                    health--;
                    lblHealth.Text = "Health: " + health.ToString();
                    plane_pb.Location = new Point(195, 495);
                }
            }
        }
        void gameOver()
        {
            ballMover.Stop();
            planeMover.Stop();
            string messageToBeShown = "You Lost! :(";

            if (health == 10)
            {
                messageToBeShown = "You Won! :)";
            }

            MessageBox.Show(messageToBeShown, "Game Finished");
            Application.Exit();
        }
        void firstGroupMover()
        {
            if (firstGroupMoving)
            {
                moveBallGroup(lstFirstGroupFromLeft, lstFirstGroupFromRight);

                firstGroupAtMiddle = checkIsGroupAtMiddle(lstBalls[lstFirstGroupFromLeft[0]]);

                firstGroupAtEnd = checkIsGroupAtEnd(lstBalls[lstFirstGroupFromLeft[0]], lstBalls[lstFirstGroupFromRight[0]]);

                if (firstGroupAtEnd)
                {
                    restartBallGroup(lstFirstGroupFromLeft);
                    restartBallGroup(lstFirstGroupFromRight);

                    firstGroupAtEnd = false;
                    firstGroupMoving = false;
                }
            }
        }

        void secondGroupMover()
        {
            if (secondGroupMoving)
            {
                moveBallGroup(lstSecondGroupFromLeft, lstSecondGroupFromRight);

                secondGroupAtMiddle = checkIsGroupAtMiddle(lstBalls[lstSecondGroupFromLeft[0]]);

                secondGroupAtEnd = checkIsGroupAtEnd(lstBalls[lstSecondGroupFromLeft[0]], lstBalls[lstSecondGroupFromRight[0]]);

                if (secondGroupAtEnd)
                {
                    restartBallGroup(lstSecondGroupFromLeft);
                    restartBallGroup(lstSecondGroupFromRight);

                    secondGroupAtEnd = false;
                    secondGroupMoving = false;
                }
            }
        }
        void thirdGroupMover()
        {
            if (thirdGroupMoving)
            {
                moveBallGroup(lstThirdGroupFromLeft, lstThirdGroupFromRight);

                thirdGroupAtMiddle = checkIsGroupAtMiddle(lstBalls[lstThirdGroupFromLeft[0]]);

                thirdGroupAtEnd = checkIsGroupAtEnd(lstBalls[lstThirdGroupFromLeft[0]], lstBalls[lstThirdGroupFromRight[0]]);

                if (thirdGroupAtEnd)
                {
                    restartBallGroup(lstThirdGroupFromLeft);
                    restartBallGroup(lstThirdGroupFromRight);

                    thirdGroupAtEnd = false;
                    thirdGroupMoving = false;
                }
            }
        }
        void moveBallGroup(List<int> lstLeft, List<int> lstRight)
        {
            int locX = 0, locY = 0;
            for (int i = 0; i < lstLeft.Count; i++)
            {
                locX = lstBalls[lstLeft[i]].Location.X;
                locY = lstBalls[lstLeft[i]].Location.Y;

                lstBalls[lstLeft[i]].Location = new Point(locX + ballVelocity, locY);
            }

            for (int i = 0; i < lstRight.Count; i++)
            {
                locX = lstBalls[lstRight[i]].Location.X;
                locY = lstBalls[lstRight[i]].Location.Y;

                lstBalls[lstRight[i]].Location = new Point(locX - ballVelocity, locY);
            }
        }
        private bool checkIsGroupAtMiddle(PictureBox ballOnLeft)
        {
            bool statusMiddle = false;
            if (ballOnLeft.Location.X >= 200)
            {
                statusMiddle = true;
            }
            return statusMiddle;
        }
        private bool checkIsGroupAtEnd(PictureBox ballOnLeft, PictureBox ballOnRight)
        {
            bool statusEnd = false;
            if (ballOnLeft.Location.X >= 400 && ballOnRight.Location.X <= 100)
            {
                statusEnd = true;
            }
            return statusEnd;
        }
        void restartBallGroup(List<int> receivedGroup)
        {
            for (int i = 0; i < receivedGroup.Count; i++)
            {
                lstBalls[receivedGroup[i]].Location = new Point(lstBallsStartLocX[receivedGroup[i]], lstBalls[receivedGroup[i]].Location.Y);

                lstIsBallMoving[receivedGroup[i]] = false;
            }
            receivedGroup.Clear();
            receivedGroup.TrimExcess();
        }
        private void planeMover_Tick(object sender, EventArgs e)
        {
            upDownMovement();

            checkPlaneCrossedOver();
        }
        void upDownMovement()
        {
            if (plane_pb.Location.Y < 496 && plane_pb.Location.Y > -40)
            {
                if (isPushedUp)
                {
                    plane_pb.Location = new Point(plane_pb.Location.X, plane_pb.Location.Y - 2);
                }
                
                else if (isPushedDown)
                {
                    plane_pb.Location = new Point(plane_pb.Location.X, plane_pb.Location.Y + 2);

                    if (plane_pb.Location.Y > 495)
                    {
                        plane_pb.Location = new Point(plane_pb.Location.X, 495);
                    }
                }
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                isPushedUp = true;
            }
            if (e.KeyCode == Keys.S)
            {
                isPushedDown = true;
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            isPushedUp = false;
            isPushedDown = false;
        }
        void checkPlaneCrossedOver()
        {
            if (plane_pb.Location.Y < -38)
            {
                health++;
                lblHealth.Text = "Health: " + health.ToString();
                plane_pb.Location = new Point(195, 495);
            }
        }
    }
}
