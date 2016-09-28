using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using MyCinema.Models;

namespace MyCinema
{
    public partial class MainForm : Form
    {
        Cinema cinema;
        Dictionary<string, Label> labels = new Dictionary<string, Label>();
        int ticket = 0;
        string key = null;
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.lblActor.Text = "";
            this.lblDirector.Text = "";
            this.lblMovieName.Text = "";
            this.lblPrice.Text = "";
            this.lblTime.Text = "";
            this.lblType.Text = "";
            this.lblCalcPrice.Text = "";
            this.txtCustomer.Enabled = false;
            this.cmbDisCount.Enabled = false;
            this.rdoNormal.Checked = true;

            cinema = new Cinema();
            //��ʼ����ӳ����λ
            InitSeats(7, 5, tpCinema);
            

            cinema.Load();
        }

        /// <summary>
        /// ��ʼ����ӳ����λ
        /// </summary>
        /// <param name="seatRow">����</param>
        /// <param name="seatLine">����</param>
        /// <param name="tb"></param>
        private void InitSeats(int seatRow,int seatLine,TabPage tb)
        {
            Label label;
            Seat seat;
            for (int i = 0; i < seatRow; i++)
            {
                for (int j = 0; j < seatLine; j++)
                {
                    label = new Label();
                    //���ñ�����ɫ
                    label.BackColor = Color.Yellow;
                    //��������
                    label.Font = new System.Drawing.Font("����", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,((byte)(134)));
                    //���óߴ�
                    label.AutoSize = false;
                    label.Size = new System.Drawing.Size(50, 25);
                    //������λ��
                    label.Text = (j + 1).ToString() + "-" + (i + 1).ToString();
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    //����λ��
                    label.Location = new Point(60 + (i * 90), 60 + (j * 60));
                    //���еı�ǩ���󶨵�ͬһ�¼�
                    label.Click += new System.EventHandler(lblSeat_Click);
                    tb.Controls.Add(label);
                    labels.Add(label.Text, label);
                    //ʵ����һ����λ
                    seat = new Seat((j + 1).ToString() + "-" + (i + 1).ToString(), Color.Yellow);
                    //�������λ����
                    cinema.Seats.Add(seat.SeatNum, seat);
                }
            }
        }

        //ѡ�񡰼������ۡ�
        private void tsmiMovies_Click(object sender, EventArgs e)
        {
            //�жϷ�ӳ�б��Ƿ�Ϊ��
            if (cinema.Schedule.Items.Count == 0)
            {
                cinema.Schedule.LoadItems();
            }
            InitTreeView();
        }

        //ѡ�񡰻�ȡ���²����б�
        private void tsmiNew_Click(object sender, EventArgs e)
        {
            cinema.Schedule.LoadItems();
            cinema.SoldTickets.Clear();
            InitTreeView();
        } 

        /// <summary>
        /// ��ʼ��TreeView�ؼ�
        /// </summary>
        private void InitTreeView()
        {
            tvMovies.BeginUpdate();
            tvMovies.Nodes.Clear();

            string movieName = null;
            TreeNode movieNode = null;
            foreach (ScheduleItem item in cinema.Schedule.Items.Values)
            {
                if (movieName != item.Movie.MovieName)
                {
                    movieNode = new TreeNode(item.Movie.MovieName);
                    tvMovies.Nodes.Add(movieNode);
                }
                TreeNode timeNode = new TreeNode(item.Time);
                movieNode.Nodes.Add(timeNode);
                movieName = item.Movie.MovieName;

            }
            tvMovies.EndUpdate();
        }

        /// <summary>
        /// ѡ��һ����Ӱ�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvMovies_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = tvMovies.SelectedNode;
            if (node == null) return;
            if (node.Level != 1) return;
            key = node.Text;
            //����ϸ��Ϣ��ʾ
            this.lblMovieName.Text = cinema.Schedule.Items[key].Movie.MovieName;
            this.lblDirector.Text = cinema.Schedule.Items[key].Movie.Director;
            this.lblActor.Text = cinema.Schedule.Items[key].Movie.Actor;
            this.lblPrice.Text = cinema.Schedule.Items[key].Movie.Price.ToString();
            this.lblTime.Text = cinema.Schedule.Items[key].Time;
            this.lblType.Text = cinema.Schedule.Items[key].Movie.MovieType.ToString();
            this.picMovie.Image = Image.FromFile(cinema.Schedule.Items[key].Movie.Poster);
            this.lblCalcPrice.Text = "";

            //�����λ
            ClearSeat();
            //�����ó���Ӱ����λ�������
            foreach (Ticket ticket in cinema.SoldTickets)
            {
                foreach (Seat seat in cinema.Seats.Values)
                {
                    if ((ticket.ScheduleItem.Time == key)
                        &&(ticket.Seat.SeatNum == seat.SeatNum))
                    {
                        seat.Color = Color.Red;
                    }
                }
            }
            UpdateSeat();
        }
        
        /// <summary>
        /// �����λ
        /// </summary>
        private void ClearSeat()
        {
            foreach (Seat seat in cinema.Seats.Values)
            {
                seat.Color = Color.Yellow;
            }
        }
        /// <summary>
        /// ������λ״̬ 
        /// </summary>
        private void UpdateSeat()
        {
            foreach (string key in cinema.Seats.Keys)
            {
                labels[key].BackColor = cinema.Seats[key].Color;
            }
        }
        /// <summary>
        /// ���һ����λ
        /// ��Ʊ�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblSeat_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.lblMovieName.Text))
            {
                MessageBox.Show("����ûѡ���Ӱ!","��ʾ");
                return;
            }
            ticket++;
            try
            {
                string seatNum = ((Label)sender).Text.ToString();
                string customerName = this.txtCustomer.Text.ToString();
                int discount = 0;
                string type = "";
                if (this.rdoStudent.Checked)
                {
                    type = "student";
                    if (this.cmbDisCount.Text == null)
                    {
                        MessageBox.Show("�������ۿ���!","��ʾ");
                        return;
                    }
                    else
                    {
                        discount = int.Parse(this.cmbDisCount.Text);
                    }
                }
                else if (this.rdoFree.Checked)
                {
                    if (String.IsNullOrEmpty(this.txtCustomer.Text))
                    {
                        MessageBox.Show("��������Ʊ������!","��ʾ");
                        return;
                    }
                    else
                    {
                        type = "free";
                    }
                }
                
                           
                //���ù�������Ʊ
                Ticket newTicket = TicketFactory.CreateTicket(cinema.Schedule.Items[key], cinema.Seats[seatNum],
                    discount, customerName, type);
                if (cinema.Seats[seatNum].Color == Color.Yellow)
                {
                    //��ӡ
                    DialogResult result;
                    result = MessageBox.Show("�Ƿ���?", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        cinema.Seats[seatNum].Color = Color.Red;
                        UpdateSeat();
                        cinema.SoldTickets.Add(newTicket);
                        newTicket.CalcPrice();
                        lblCalcPrice.Text = newTicket.Price.ToString();
                        newTicket.Print();
                    }
                    else if (result == DialogResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("���۳�.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //ѡ����Ʊ��ʱ
        private void rdoFree_CheckedChanged(object sender, EventArgs e)
        {
            this.txtCustomer.Enabled = true;
            this.cmbDisCount.Enabled = false;
            this.cmbDisCount.Text = "";
            //���á��Żݼۡ�
            this.lblCalcPrice.Text = "0";
        }

        //ѡ��ѧ��Ʊ��ʱ
        private void rdoStudent_CheckedChanged(object sender, EventArgs e)
        {
            this.txtCustomer.Enabled = false;
            this.txtCustomer.Text = "";
            this.cmbDisCount.Enabled = true;
            this.cmbDisCount.Text = "7";
            //���ݵ�ǰѡ�еĵ�Ӱ�����á��Żݼۡ�
            if(this.lblPrice.Text!="")
            {
                int price = int.Parse(this.lblPrice.Text);
                int discount = int.Parse(this.cmbDisCount.Text);
                this.lblCalcPrice.Text = (price * discount / 10).ToString();
            }
            
        }

        //ѡ����ͨƱ��ʱ
        private void rdoNormal_CheckedChanged(object sender, EventArgs e)
        {
            this.cmbDisCount.Enabled = false;
            this.txtCustomer.Text = "";
            this.txtCustomer.Enabled = false;
            this.cmbDisCount.Text = "";
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            //�˳�ʱ���л�Cinema����
            cinema.Save();
            this.Dispose();
        }

        private void tsmiSave_Click(object sender, EventArgs e)
        {
            cinema.Save();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult close;
            close = MessageBox.Show("�Ƿ񱣴浱ǰ����״̬?", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (close == DialogResult.Yes)
            {
                cinema.Save();
            }
        }

        //ѡ�񡰲�ͬ�ۿۡ������б�
        private void cmbDisCount_SelectedIndexChanged(object sender, EventArgs e)
        {
            //���ݵ�ǰѡ�еĵ�Ӱ�����á��Żݼۡ�
            if (this.lblPrice.Text != "")
            {
                int price = int.Parse(this.lblPrice.Text);
                int discount = int.Parse(this.cmbDisCount.Text);
                this.lblCalcPrice.Text = (price * discount / 10).ToString();
            }
        }

        
    }
}