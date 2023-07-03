using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Media;
using NAudio.Wave;


namespace shooting_zombie
{
    public partial class Form1 : Form
    {
        bool goLeft, goRight, goUp, goDown, gameOver;
       // bool shoot, dead, walk, getammo;
        string facing = "up";
        int speed = 10;
        int ammo = 10;
        int zombieSpeed = 1;
        int score;
        int playerHealth = 100;
        Random random = new Random();
        List<PictureBox> zombiesList = new List<PictureBox>();


        public Form1()
        {
            InitializeComponent();
            RestartGame();
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            if (playerHealth > 1)
            {
                healthBar.Value = playerHealth;
            }
            else
            {
                gameOver = true;
                Player.Image = Properties.Resources.dead;
                GameTimer.Stop();
                PlayMp3("man_scream.mp3", 500);

            }

            TextAmmo.Text = "ຈຳນວນລູກປືນ: " + ammo;
            TextScore.Text = "ຈຳນວນທີ່ຂ້າ: " + score;

            // move left,right,to,bottom

            if (goLeft == true && Player.Left > 0)
            {
                Player.Left -= speed;
            }
            if (goRight == true && Player.Left + Player.Width < this.ClientSize.Width)
            {
                Player.Left += speed;
            }
            if (goUp == true && Player.Top > 45)
            {
                Player.Top -= speed;
            }
            if (goDown == true && Player.Top + Player.Height < this.ClientSize.Height)
            {
                Player.Top += speed;
            }
            // checking ammo if plyer hit then it was collected then it was removed from screen

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    if (Player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        ammo += 5;
                        // sound to add ammo

                        PlayMp3("gotammo.mp3", 200);



                    }
                }
                if (x is PictureBox && (string)x.Tag == "zombie")
                {
                    if (Player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerHealth -= 1;
                        
                    }


                    if (x.Left > Player.Left)
                    {
                        x.Left -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zleft;
                        
                    }
                    if (x.Left < Player.Left)
                    {
                        x.Left += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zright;
                        
                    }
                    if (x.Top > Player.Top)
                    {
                        x.Top -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zup;
                        
                    }
                    if (x.Top < Player.Top)
                    {
                        x.Top += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zdown;
                        
                    }
                    // somthing...

                    foreach (Control j in this.Controls)
                    {
                        if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "zombie")
                        {
                            if (x.Bounds.IntersectsWith(j.Bounds))
                            {
                                score++;

                                PlayMp3("zombie-growl.mp3", 400);

                                this.Controls.Remove(j);
                                ((PictureBox)j).Dispose();
                                this.Controls.Remove(x);
                                ((PictureBox)x).Dispose();
                                zombiesList.Remove(((PictureBox)x));
                               
                                MakeZombies();
                             
                               
                            }
                        }
                    }
                }
            }



        }

        private void KeyisDown(object sender, KeyEventArgs e)
        {

            if (gameOver == true)
            {
              
                return;  // Game over 
            }

            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
                facing = "left";
                Player.Image = Properties.Resources.left;
              

            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
                facing = "right";
                Player.Image = Properties.Resources.right;
           

            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
                facing = "up";
                Player.Image = Properties.Resources.up;

            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
                facing = "down";
               // RunSound();
                Player.Image = Properties.Resources.down;
              

            }

        }

        private void KeyisUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
              
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
               
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
               
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
               
            }

            // press space on ket board for shooting 

            if (e.KeyCode == Keys.Space && ammo > 0 && gameOver == false)
            {
                ammo--;
                ShootBullet(facing);
                // shoot sound

                PlayMp3("fire_sound.mp3", 200);

                if (ammo < 1)
                {
                    DropAmmo();
                    PlayMp3("success.mp3",300);
                   

                }
            }


            // press enter to restart game 
            if (e.KeyCode == Keys.Enter && gameOver == true)
            {  
                RestartGame();
            }

        }
        private void ShootBullet(string directtion)
        {     

            Bullet shootBullet = new Bullet();
            shootBullet.direction = directtion;
            shootBullet.bulletLeft = Player.Left + (Player.Width / 2);
            shootBullet.bulletTop = Player.Top + (Player.Height / 2);
            shootBullet.MakeBullet(this);
        

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Main_form main = new Main_form();
            this.Hide();
            main.Show();
        }

        private void MakeZombies()
        {
            PictureBox zombie = new PictureBox();
            zombie.Tag = "zombie";
            zombie.Image = Properties.Resources.zdown;
            zombie.Top = random.Next(0, 800);
            zombie.Left = random.Next(0, 900);
            zombie.SizeMode = PictureBoxSizeMode.AutoSize;
            zombiesList.Add(zombie);
            this.Controls.Add(zombie);
            Player.BringToFront();
            // call shoot function

        }
        private void DropAmmo()
        {
            PictureBox ammo = new PictureBox();
            ammo.Image = Properties.Resources.ammo_Image;
            ammo.SizeMode = PictureBoxSizeMode.AutoSize;
            ammo.Left = random.Next(10, this.ClientSize.Width - ammo.Width);
            ammo.Top = random.Next(60, this.ClientSize.Height - ammo.Height);
            ammo.Tag = "ammo";
            this.Controls.Add(ammo);

            ammo.BringToFront();
            Player.BringToFront();
           


        }
        private void RestartGame()
        {
            Player.Image = Properties.Resources.up;

            foreach (PictureBox i in zombiesList)
            {
                this.Controls.Remove(i);
            }

            zombiesList.Clear();

            for (int i = 1; i < 3; i++)
            {
                MakeZombies();
            }

            goUp = false;
            goDown = false;
            goLeft = false;
            goRight = false;
            gameOver = false;
            

            playerHealth = 100;
            score = 0;
            ammo = 10;

            GameTimer.Start();
        }

        

        
        // shoot function sound
        public  void PlayMp3(string audioFile ,int time)
        {
            var reader = new Mp3FileReader(audioFile);
            var waveOut = new WaveOutEvent();
            waveOut.Init(reader);
            waveOut.Play();
            System.Threading.Thread.Sleep(time);
            waveOut.Stop();
            waveOut.Dispose();
            reader.Dispose();
        }
        // got atack 

    }
    }
