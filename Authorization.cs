using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;
using System.Security.Cryptography;

namespace Diplom
{
    public partial class Authorization : MaterialForm
    {
        public Authorization()
        {
            InitializeComponent();
            RegistrationPANEL.Hide();
            // MaterialSkin
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        public bool userExist = false;


        private void RegistrationBTN_Click(object sender, EventArgs e)
        {
            AuthorizationPANEL.Hide();
            RegistrationPANEL.Show();
        }

        private void EnterBTN_Click(object sender, EventArgs e)
        {
            Authorizate();
        }
        private void RegisterNewUserBTN_Click(object sender, EventArgs e)
        {
            Registration();
            AuthorizationPANEL.Show();
            RegistrationPANEL.Hide();
        }
        private string LoadData(string sql) // Запрос данных
        {
            SqlConnection con = new SqlConnection("Data Source=DESKTOP-LIU1R6H; Initial Catalog = Security; " + "Integrated Security=True;");
            con.Open();
            string sqlcom = sql;
            SqlCommand cmd = new SqlCommand(sqlcom, con);
            object data = cmd.ExecuteScalar();
            if (data != null)
            {
                string udata = data.ToString();
                con.Close();
                return udata;
            }
            else
            {
                con.Close();
                MessageBox.Show("Информация не найдена в базе данных");
            }
            return "null";
        }
        
        private void Authorizate()
        {
            if (!string.IsNullOrWhiteSpace(LoginTB.Text) && !string.IsNullOrWhiteSpace(PasswordTB.Text))
            {
                string username = LoginTB.Text;
                string password = PasswordTB.Text;
                string usernameBD = LoadData("SELECT userName FROM [User] WHERE userName ='" + username + "'").Replace(" ", "");
                string passwordBD = LoadData("SELECT userPassword FROM [User] WHERE userName ='" + username + "'").Replace(" ", "");
                if ((username == usernameBD) && (Encode(password) == passwordBD))
                {
                    this.Hide();
                    Form mainmenu = new MainMenu(username);
                    mainmenu.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверные данные для входа!");
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля!");
            }
        }

        private void Registration()
        {
            string username = LoginRegTB.Text;
            string password = PasswordRegTB.Text;
            string passwordRepeat = RepeatPasswordTB.Text;
            if (!string.IsNullOrWhiteSpace(LoginRegTB.Text) && !string.IsNullOrWhiteSpace(PasswordRegTB.Text) && !string.IsNullOrWhiteSpace(RepeatPasswordTB.Text))
            {
                if (password != passwordRepeat) MessageBox.Show("Пароли отличаются!");
                SqlConnection con = new SqlConnection("Data Source=DESKTOP-LIU1R6H; Initial Catalog = Security; " + "Integrated Security=True;");
                con.Open();
                string sqlcom = $"INSERT INTO [User] (userName, userPassword, userRole) VALUES ('{username}', '{Encode(password)}', 'Operator')";
                SqlCommand cmd = new SqlCommand(sqlcom, con);
                object data = cmd.ExecuteNonQuery();
                con.Close();
            }
            else
            {
                MessageBox.Show("Заполните все поля!");
            }
        }
        private string Encode(string s)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] inputBytes = Encoding.UTF32.GetBytes(s);
            byte[] hashBytes = sha256.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        // LOGGER CODE BELOW
        private void logger_new(string some_info)
        {
            //Logger.WriteLog("[Таблица '" + name + "' - " + action_name + "] Изменена строка: id='" + textID.Text + "'");
            Logger.WriteLog(some_info);
        }



        private void InitializeComponent()
        {
            this.LoginTB = new MaterialSkin.Controls.MaterialTextBox2();
            this.PasswordTB = new MaterialSkin.Controls.MaterialTextBox2();
            this.EnterBTN = new MaterialSkin.Controls.MaterialButton();
            this.RegistrationBTN = new MaterialSkin.Controls.MaterialButton();
            this.AuthorizationPANEL = new System.Windows.Forms.Panel();
            this.RegistrationPANEL = new System.Windows.Forms.Panel();
            this.RepeatPasswordTB = new MaterialSkin.Controls.MaterialTextBox2();
            this.LoginRegTB = new MaterialSkin.Controls.MaterialTextBox2();
            this.RegisterNewUserBTN = new MaterialSkin.Controls.MaterialButton();
            this.PasswordRegTB = new MaterialSkin.Controls.MaterialTextBox2();
            this.AuthorizationPANEL.SuspendLayout();
            this.RegistrationPANEL.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoginTB
            // 
            this.LoginTB.AnimateReadOnly = false;
            this.LoginTB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.LoginTB.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.LoginTB.Depth = 0;
            this.LoginTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.LoginTB.HideSelection = true;
            this.LoginTB.Hint = "Логин";
            this.LoginTB.LeadingIcon = null;
            this.LoginTB.Location = new System.Drawing.Point(3, 3);
            this.LoginTB.MaxLength = 32767;
            this.LoginTB.MouseState = MaterialSkin.MouseState.OUT;
            this.LoginTB.Name = "LoginTB";
            this.LoginTB.PasswordChar = '\0';
            this.LoginTB.PrefixSuffixText = null;
            this.LoginTB.ReadOnly = false;
            this.LoginTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LoginTB.SelectedText = "";
            this.LoginTB.SelectionLength = 0;
            this.LoginTB.SelectionStart = 0;
            this.LoginTB.ShortcutsEnabled = true;
            this.LoginTB.Size = new System.Drawing.Size(250, 48);
            this.LoginTB.TabIndex = 1;
            this.LoginTB.TabStop = false;
            this.LoginTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.LoginTB.TrailingIcon = null;
            this.LoginTB.UseSystemPasswordChar = false;
            // 
            // PasswordTB
            // 
            this.PasswordTB.AnimateReadOnly = false;
            this.PasswordTB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PasswordTB.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.PasswordTB.Depth = 0;
            this.PasswordTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.PasswordTB.HideSelection = true;
            this.PasswordTB.Hint = "Пароль";
            this.PasswordTB.LeadingIcon = null;
            this.PasswordTB.Location = new System.Drawing.Point(3, 57);
            this.PasswordTB.MaxLength = 32767;
            this.PasswordTB.MouseState = MaterialSkin.MouseState.OUT;
            this.PasswordTB.Name = "PasswordTB";
            this.PasswordTB.PasswordChar = '\0';
            this.PasswordTB.PrefixSuffixText = null;
            this.PasswordTB.ReadOnly = false;
            this.PasswordTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PasswordTB.SelectedText = "";
            this.PasswordTB.SelectionLength = 0;
            this.PasswordTB.SelectionStart = 0;
            this.PasswordTB.ShortcutsEnabled = true;
            this.PasswordTB.Size = new System.Drawing.Size(250, 48);
            this.PasswordTB.TabIndex = 2;
            this.PasswordTB.TabStop = false;
            this.PasswordTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.PasswordTB.TrailingIcon = null;
            this.PasswordTB.UseSystemPasswordChar = false;
            // 
            // EnterBTN
            // 
            this.EnterBTN.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.EnterBTN.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.EnterBTN.Depth = 0;
            this.EnterBTN.HighEmphasis = true;
            this.EnterBTN.Icon = null;
            this.EnterBTN.Location = new System.Drawing.Point(182, 114);
            this.EnterBTN.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.EnterBTN.MouseState = MaterialSkin.MouseState.HOVER;
            this.EnterBTN.Name = "EnterBTN";
            this.EnterBTN.NoAccentTextColor = System.Drawing.Color.Empty;
            this.EnterBTN.Size = new System.Drawing.Size(71, 36);
            this.EnterBTN.TabIndex = 3;
            this.EnterBTN.Text = "ВОЙТИ";
            this.EnterBTN.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.EnterBTN.UseAccentColor = false;
            this.EnterBTN.UseVisualStyleBackColor = true;
            this.EnterBTN.Click += new System.EventHandler(this.EnterBTN_Click);
            // 
            // RegistrationBTN
            // 
            this.RegistrationBTN.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.RegistrationBTN.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.RegistrationBTN.Depth = 0;
            this.RegistrationBTN.HighEmphasis = true;
            this.RegistrationBTN.Icon = null;
            this.RegistrationBTN.Location = new System.Drawing.Point(4, 114);
            this.RegistrationBTN.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.RegistrationBTN.MouseState = MaterialSkin.MouseState.HOVER;
            this.RegistrationBTN.Name = "RegistrationBTN";
            this.RegistrationBTN.NoAccentTextColor = System.Drawing.Color.Empty;
            this.RegistrationBTN.Size = new System.Drawing.Size(126, 36);
            this.RegistrationBTN.TabIndex = 4;
            this.RegistrationBTN.Text = "РЕГИСТРАЦИЯ";
            this.RegistrationBTN.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.RegistrationBTN.UseAccentColor = false;
            this.RegistrationBTN.UseVisualStyleBackColor = true;
            this.RegistrationBTN.Click += new System.EventHandler(this.RegistrationBTN_Click);
            // 
            // AuthorizationPANEL
            // 
            this.AuthorizationPANEL.Controls.Add(this.LoginTB);
            this.AuthorizationPANEL.Controls.Add(this.RegistrationBTN);
            this.AuthorizationPANEL.Controls.Add(this.PasswordTB);
            this.AuthorizationPANEL.Controls.Add(this.EnterBTN);
            this.AuthorizationPANEL.Location = new System.Drawing.Point(6, 75);
            this.AuthorizationPANEL.Name = "AuthorizationPANEL";
            this.AuthorizationPANEL.Size = new System.Drawing.Size(258, 155);
            this.AuthorizationPANEL.TabIndex = 5;
            // 
            // RegistrationPANEL
            // 
            this.RegistrationPANEL.Controls.Add(this.RepeatPasswordTB);
            this.RegistrationPANEL.Controls.Add(this.LoginRegTB);
            this.RegistrationPANEL.Controls.Add(this.RegisterNewUserBTN);
            this.RegistrationPANEL.Controls.Add(this.PasswordRegTB);
            this.RegistrationPANEL.Location = new System.Drawing.Point(6, 75);
            this.RegistrationPANEL.Name = "RegistrationPANEL";
            this.RegistrationPANEL.Size = new System.Drawing.Size(258, 215);
            this.RegistrationPANEL.TabIndex = 6;
            // 
            // RepeatPasswordTB
            // 
            this.RepeatPasswordTB.AnimateReadOnly = false;
            this.RepeatPasswordTB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.RepeatPasswordTB.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.RepeatPasswordTB.Depth = 0;
            this.RepeatPasswordTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.RepeatPasswordTB.HideSelection = true;
            this.RepeatPasswordTB.Hint = "Повторите пароль";
            this.RepeatPasswordTB.LeadingIcon = null;
            this.RepeatPasswordTB.Location = new System.Drawing.Point(4, 113);
            this.RepeatPasswordTB.MaxLength = 32767;
            this.RepeatPasswordTB.MouseState = MaterialSkin.MouseState.OUT;
            this.RepeatPasswordTB.Name = "RepeatPasswordTB";
            this.RepeatPasswordTB.PasswordChar = '\0';
            this.RepeatPasswordTB.PrefixSuffixText = null;
            this.RepeatPasswordTB.ReadOnly = false;
            this.RepeatPasswordTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.RepeatPasswordTB.SelectedText = "";
            this.RepeatPasswordTB.SelectionLength = 0;
            this.RepeatPasswordTB.SelectionStart = 0;
            this.RepeatPasswordTB.ShortcutsEnabled = true;
            this.RepeatPasswordTB.Size = new System.Drawing.Size(250, 48);
            this.RepeatPasswordTB.TabIndex = 5;
            this.RepeatPasswordTB.TabStop = false;
            this.RepeatPasswordTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.RepeatPasswordTB.TrailingIcon = null;
            this.RepeatPasswordTB.UseSystemPasswordChar = false;
            // 
            // LoginRegTB
            // 
            this.LoginRegTB.AnimateReadOnly = false;
            this.LoginRegTB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.LoginRegTB.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.LoginRegTB.Depth = 0;
            this.LoginRegTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.LoginRegTB.HideSelection = true;
            this.LoginRegTB.Hint = "Логин";
            this.LoginRegTB.LeadingIcon = null;
            this.LoginRegTB.Location = new System.Drawing.Point(4, 5);
            this.LoginRegTB.MaxLength = 32767;
            this.LoginRegTB.MouseState = MaterialSkin.MouseState.OUT;
            this.LoginRegTB.Name = "LoginRegTB";
            this.LoginRegTB.PasswordChar = '\0';
            this.LoginRegTB.PrefixSuffixText = null;
            this.LoginRegTB.ReadOnly = false;
            this.LoginRegTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LoginRegTB.SelectedText = "";
            this.LoginRegTB.SelectionLength = 0;
            this.LoginRegTB.SelectionStart = 0;
            this.LoginRegTB.ShortcutsEnabled = true;
            this.LoginRegTB.Size = new System.Drawing.Size(250, 48);
            this.LoginRegTB.TabIndex = 6;
            this.LoginRegTB.TabStop = false;
            this.LoginRegTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.LoginRegTB.TrailingIcon = null;
            this.LoginRegTB.UseSystemPasswordChar = false;
            // 
            // RegisterNewUserBTN
            // 
            this.RegisterNewUserBTN.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.RegisterNewUserBTN.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.RegisterNewUserBTN.Depth = 0;
            this.RegisterNewUserBTN.HighEmphasis = true;
            this.RegisterNewUserBTN.Icon = null;
            this.RegisterNewUserBTN.Location = new System.Drawing.Point(52, 170);
            this.RegisterNewUserBTN.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.RegisterNewUserBTN.MouseState = MaterialSkin.MouseState.HOVER;
            this.RegisterNewUserBTN.Name = "RegisterNewUserBTN";
            this.RegisterNewUserBTN.NoAccentTextColor = System.Drawing.Color.Empty;
            this.RegisterNewUserBTN.Size = new System.Drawing.Size(160, 36);
            this.RegisterNewUserBTN.TabIndex = 9;
            this.RegisterNewUserBTN.Text = "Создать аккаунт";
            this.RegisterNewUserBTN.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.RegisterNewUserBTN.UseAccentColor = false;
            this.RegisterNewUserBTN.UseVisualStyleBackColor = true;
            this.RegisterNewUserBTN.Click += new System.EventHandler(this.RegisterNewUserBTN_Click);
            // 
            // PasswordRegTB
            // 
            this.PasswordRegTB.AnimateReadOnly = false;
            this.PasswordRegTB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.PasswordRegTB.CharacterCasing = System.Windows.Forms.CharacterCasing.Normal;
            this.PasswordRegTB.Depth = 0;
            this.PasswordRegTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.PasswordRegTB.HideSelection = true;
            this.PasswordRegTB.Hint = "Пароль";
            this.PasswordRegTB.LeadingIcon = null;
            this.PasswordRegTB.Location = new System.Drawing.Point(4, 59);
            this.PasswordRegTB.MaxLength = 32767;
            this.PasswordRegTB.MouseState = MaterialSkin.MouseState.OUT;
            this.PasswordRegTB.Name = "PasswordRegTB";
            this.PasswordRegTB.PasswordChar = '\0';
            this.PasswordRegTB.PrefixSuffixText = null;
            this.PasswordRegTB.ReadOnly = false;
            this.PasswordRegTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PasswordRegTB.SelectedText = "";
            this.PasswordRegTB.SelectionLength = 0;
            this.PasswordRegTB.SelectionStart = 0;
            this.PasswordRegTB.ShortcutsEnabled = true;
            this.PasswordRegTB.Size = new System.Drawing.Size(250, 48);
            this.PasswordRegTB.TabIndex = 7;
            this.PasswordRegTB.TabStop = false;
            this.PasswordRegTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.PasswordRegTB.TrailingIcon = null;
            this.PasswordRegTB.UseSystemPasswordChar = false;
            // 
            // Authorization
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(269, 296);
            this.Controls.Add(this.RegistrationPANEL);
            this.Controls.Add(this.AuthorizationPANEL);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.FormStyle = MaterialSkin.Controls.MaterialForm.FormStyles.ActionBar_48;
            this.Name = "Authorization";
            this.Padding = new System.Windows.Forms.Padding(3, 72, 3, 3);
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Авторизация";
            this.AuthorizationPANEL.ResumeLayout(false);
            this.AuthorizationPANEL.PerformLayout();
            this.RegistrationPANEL.ResumeLayout(false);
            this.RegistrationPANEL.PerformLayout();
            this.ResumeLayout(false);

        }

    }
}
