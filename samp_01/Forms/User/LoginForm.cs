using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using samp_01.Services.User;
using samp_01.Domain.DTO;

namespace samp_01.Forms.User
{
    public class LoginForm : Form
    {
        private TextBox txtName;
        private TextBox txtPassword;
        private TextBox txtEmail;
        private TextBox txtCompany;
        private Button btnLogin;
        private Button btnRegister;
        private Label lblMessage;
        private ComboBox cboRole;
        private readonly LoginService _loginService;
        private Panel card;
        private Label lblName, lblEmail, lblCompany, lblPass;

        public LoginForm()
        {
            _loginService = new LoginService();

            var accent = Color.FromArgb(10, 132, 255);
            var background = Color.FromArgb(250, 250, 252);
            var cardColor = Color.White;
            var textColor = Color.FromArgb(34, 34, 34);
            var borderColor = Color.FromArgb(230, 230, 230);

            Text = "Login";
            ClientSize = new Size(520, 400);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = background;
            Font = new Font("Segoe UI", 9);
            Padding = new Padding(20);

            // Header section
            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.Transparent };
            var lblTitle = new Label { Text = "Welcome", Font = new Font("Segoe UI", 24, FontStyle.Bold), Left = 0, Top = 10, AutoSize = true, ForeColor = textColor };
            var lblSubtitle = new Label { Text = "Sign in to your account", Font = new Font("Segoe UI", 11), Left = 0, Top = 50, AutoSize = true, ForeColor = Color.FromArgb(100, 100, 100) };
            headerPanel.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });

            // Main card
            card = new Panel
            {
                Size = new Size(460, 280),
                Left = 30,
                Top = 100,
                BackColor = cardColor,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(25)
            };

            // Role selection
            var rolePanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.Transparent };
            var lblRole = new Label { Text = "Role", Dock = DockStyle.Left, Width = 80, ForeColor = textColor, Font = new Font("Segoe UI", 9, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
            cboRole = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9),
                FlatStyle = FlatStyle.Flat
            };
            cboRole.Items.AddRange(new string[] { "user", "seller", "admin" });
            cboRole.SelectedIndex = 0;
            cboRole.SelectedIndexChanged += CboRole_SelectedIndexChanged;
            rolePanel.Controls.Add(cboRole);
            rolePanel.Controls.Add(lblRole);

            // Name field
            var namePanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.Transparent, Margin = new Padding(0, 10, 0, 0) };
            lblName = new Label { Text = "Name", Dock = DockStyle.Left, Width = 80, ForeColor = textColor, Font = new Font("Segoe UI", 9, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
            txtName = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };
            namePanel.Controls.Add(txtName);
            namePanel.Controls.Add(lblName);

            // Email field
            var emailPanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.Transparent, Margin = new Padding(0, 10, 0, 0), Visible = false };
            lblEmail = new Label { Text = "Email", Dock = DockStyle.Left, Width = 80, ForeColor = textColor, Font = new Font("Segoe UI", 9, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
            txtEmail = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };
            emailPanel.Controls.Add(txtEmail);
            emailPanel.Controls.Add(lblEmail);

            // Company field
            var companyPanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.Transparent, Margin = new Padding(0, 10, 0, 0), Visible = false };
            lblCompany = new Label { Text = "Company", Dock = DockStyle.Left, Width = 80, ForeColor = textColor, Font = new Font("Segoe UI", 9, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
            txtCompany = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle
            };
            companyPanel.Controls.Add(txtCompany);
            companyPanel.Controls.Add(lblCompany);

            // Password field
            var passwordPanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.Transparent, Margin = new Padding(0, 10, 0, 0) };
            lblPass = new Label { Text = "Password", Dock = DockStyle.Left, Width = 80, ForeColor = textColor, Font = new Font("Segoe UI", 9, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft };
            txtPassword = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };
            passwordPanel.Controls.Add(txtPassword);
            passwordPanel.Controls.Add(lblPass);

            // Message label
            lblMessage = new Label
            {
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = Color.FromArgb(220, 0, 0),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 10, 0, 0)
            };

            // Buttons panel
            var buttonsPanel = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.Transparent, Margin = new Padding(0, 15, 0, 0) };
            btnLogin = new Button
            {
                Text = "Login",
                Dock = DockStyle.Right,
                Width = 100,
                Height = 35,
                BackColor = accent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            btnRegister = new Button
            {
                Text = "Register",
                Dock = DockStyle.Right,
                Width = 100,
                Height = 35,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9),
                ForeColor = accent,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;

            buttonsPanel.Controls.Add(btnLogin);
            buttonsPanel.Controls.Add(btnRegister);

            // Add controls to card in reverse order (DockStyle.Top adds in reverse)
            card.Controls.Add(buttonsPanel);
            card.Controls.Add(lblMessage);
            card.Controls.Add(passwordPanel);
            card.Controls.Add(companyPanel);
            card.Controls.Add(emailPanel);
            card.Controls.Add(namePanel);
            card.Controls.Add(rolePanel);

            Controls.Add(headerPanel);
            Controls.Add(card);

            // Initialize the form state based on default role selection
            CboRole_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void CboRole_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var role = cboRole.SelectedItem as string;
            var isSeller = role == "seller";

            // user and admin both use Name
            lblName.Parent.Visible = txtName.Visible = !isSeller;
            lblEmail.Parent.Visible = txtEmail.Visible = isSeller;
            lblCompany.Parent.Visible = txtCompany.Visible = isSeller;

            // Clear fields when switching
            if (isSeller)
            {
                txtName.Clear();
            }
            else
            {
                txtEmail.Clear();
                txtCompany.Clear();
            }
            lblMessage.Text = string.Empty;
        }

        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            var role = cboRole.SelectedItem as string;
            if (role == "seller")
            {
                using var reg = new Forms.ServiceProvider.ProviderRegistrationForm();
                reg.ShowDialog(this);
            }
            else
            {
                using var reg = new RegisterForm();
                reg.ShowDialog(this);
            }
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            lblMessage.Text = "";
            var role = (cboRole.SelectedItem as string) ?? "user";
            var dto = new LoginDTO { Role = role, Name = txtName.Text.Trim(), Email = txtEmail.Text.Trim(), CompanyName = txtCompany.Text.Trim(), Password = txtPassword.Text };
            if (_loginService.Authenticate(dto, out var profile) && profile != null)
            {
                if (role == "seller")
                {
                    var seller = (Domain.DTO.SellerProfileDTO)profile;
                    var frm = new Forms.ServiceProvider.ServiceProviderDashboardForm(seller);
                    frm.Show(); Hide(); frm.FormClosed += (s, e2) => { Show(); };
                }
                else if (role == "admin")
                {
                    var admin = (Domain.DTO.AdminProfileDTO)profile;
                    var frm = new samp_01.Forms.Admin.AdminDashboardForm(admin);
                    frm.Show(); Hide(); frm.FormClosed += (s, e2) => { Show(); };
                }
                else
                {
                    var user = (Domain.DTO.UserProfileDTO)profile;
                    var frm = new Forms.User.DashboardForm(user);
                    frm.Show(); Hide(); frm.FormClosed += (s, e2) => { Show(); };
                }
            }
            else
            {
                lblMessage.Text = "Invalid credentials.";
            }
        }
    }
}