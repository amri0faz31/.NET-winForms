using System;
using System.Drawing;
using System.Windows.Forms;
using samp_01.Domain.DTO;
using samp_01.Data; // for AdoDbContext
using samp_01.Data.Repositories;

namespace samp_01.Forms.Admin
{
    public class AdminDashboardForm : Form
    {
        private Panel _sidebar;
        private Panel _main;
        private Button _btnHome, _btnUsers, _btnSellers, _btnLogout;

        public AdminDashboardForm(AdminProfileDTO profile)
        {
            Text = "Admin Dashboard";
            ClientSize = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;

            // Colors
            var sidebarColor = Color.FromArgb(45, 45, 48);
            var accentColor = Color.FromArgb(0, 120, 215);
            var hoverColor = Color.FromArgb(62, 62, 66);
            var headerColor = Color.FromArgb(37, 37, 38);

            _sidebar = new Panel
            {
                Left = 0,
                Top = 0,
                Width = 220,
                Height = ClientSize.Height,
                BackColor = sidebarColor
            };

            _main = new Panel
            {
                Left = 220,
                Top = 0,
                Width = ClientSize.Width - 220,
                Height = ClientSize.Height,
                BackColor = Color.White
            };

            // Header in sidebar
            var lblHeader = new Label
            {
                Text = "ADMIN PANEL",
                Left = 0,
                Top = 0,
                Width = 220,
                Height = 60,
                BackColor = headerColor,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _sidebar.Controls.Add(lblHeader);

            // Button styling
            var buttonHeight = 45;
            var startTop = 70;

            _btnHome = CreateSidebarButton("Home", startTop, buttonHeight);
            _btnHome.Click += (s, e) => ShowHome(profile);

            _btnUsers = CreateSidebarButton("Users", startTop + buttonHeight, buttonHeight);
            _btnUsers.Click += (s, e) => ShowUsers();

            _btnSellers = CreateSidebarButton("Sellers", startTop + buttonHeight * 2, buttonHeight);
            _btnSellers.Click += (s, e) => ShowSellers();

            _btnLogout = CreateSidebarButton("Logout", ClientSize.Height - 70, buttonHeight);
            _btnLogout.Click += (s, e) => Close();
            _btnLogout.BackColor = Color.FromArgb(60, 60, 60);
            _btnLogout.ForeColor = Color.FromArgb(220, 120, 120);

            _sidebar.Controls.AddRange(new Control[] { _btnHome, _btnUsers, _btnSellers, _btnLogout });
            Controls.AddRange(new Control[] { _sidebar, _main });

            ShowHome(profile);
        }

        private Button CreateSidebarButton(string text, int top, int height)
        {
            var btn = new Button
            {
                Text = text,
                Left = 0,
                Top = top,
                Width = 220,
                Height = height,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Font = new Font("Segoe UI", 9),
                ImageAlign = ContentAlignment.MiddleLeft
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(62, 62, 66);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 120, 215);

            return btn;
        }

        private void LoadView(Control c)
        {
            _main.Controls.Clear();
            _main.Controls.Add(c);
        }

        private void ShowHome(AdminProfileDTO profile)
        {
            var p = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            var welcomePanel = new Panel
            {
                Left = 30,
                Top = 30,
                Width = _main.Width - 60,
                Height = 120,
                BackColor = Color.FromArgb(248, 248, 248),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblWelcome = new Label
            {
                Text = $"Welcome, {profile.Name}",
                Left = 25,
                Top = 25,
                AutoSize = true,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48)
            };

            var lblRole = new Label
            {
                Text = "Administrator",
                Left = 25,
                Top = 60,
                AutoSize = true,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            welcomePanel.Controls.Add(lblWelcome);
            welcomePanel.Controls.Add(lblRole);
            p.Controls.Add(welcomePanel);

            var statsPanel = new Panel
            {
                Left = 30,
                Top = 170,
                Width = _main.Width - 60,
                Height = 100,
                BackColor = Color.Transparent
            };

            // Quick stats (you can populate these with real data)
            var lblStats = new Label
            {
                Text = "Quick Overview",
                Left = 0,
                Top = 0,
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48)
            };
            statsPanel.Controls.Add(lblStats);

            p.Controls.Add(statsPanel);
            LoadView(p);
        }

        private void ShowUsers()
        {
            var container = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(248, 248, 248)
            };

            var lblTitle = new Label
            {
                Text = "User Management",
                Left = 25,
                Top = 20,
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48)
            };
            headerPanel.Controls.Add(lblTitle);

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                Font = new Font("Segoe UI", 9)
            };

            // Style the grid
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            using var db = new AdoDbContext(AppConfig.ConnectionString);
            var list = db.Query("SELECT id, name, email, createdat FROM users ORDER BY createdat DESC", r => new
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                Email = r.IsDBNull(2) ? null : r.GetString(2),
                CreatedAt = r.GetDateTime(3)
            });
            grid.DataSource = list;

            container.Controls.Add(grid);
            container.Controls.Add(headerPanel);
            LoadView(container);
        }

        private void ShowSellers()
        {
            var container = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(248, 248, 248)
            };

            var lblTitle = new Label
            {
                Text = "Seller Management",
                Left = 25,
                Top = 20,
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 48)
            };
            headerPanel.Controls.Add(lblTitle);

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                Font = new Font("Segoe UI", 9)
            };

            // Style the grid
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            using var db = new AdoDbContext(AppConfig.ConnectionString);
            var list = db.Query("SELECT id, name, email, companyname, createdat FROM sellers ORDER BY createdat DESC", r => new
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                Email = r.GetString(2),
                Company = r.GetString(3),
                CreatedAt = r.GetDateTime(4)
            });
            grid.DataSource = list;

            container.Controls.Add(grid);
            container.Controls.Add(headerPanel);
            LoadView(container);
        }
    }
}