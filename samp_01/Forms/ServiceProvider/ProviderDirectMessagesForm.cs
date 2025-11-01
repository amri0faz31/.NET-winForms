using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using samp_01.Data.Repositories;
using samp_01.Domain.DTO;

namespace samp_01.Forms.ServiceProvider
{
    public class ProviderDirectMessagesForm : Form
    {
         private readonly SellerProfileDTO _me;
         private readonly IMessageRepository _messages = new AdoMessageRepository(AppConfig.ConnectionString);
         private readonly IUserRepository _users = new AdoUserRepository(AppConfig.ConnectionString);
         private Panel _host = null!;
         private ListBox _peers = null!;
         private Panel _thread = null!;
         private int? _peerId;
         private string _peerType = "User"; // default peer type for seller

        public ProviderDirectMessagesForm(SellerProfileDTO me)
        {
             _me = me;
             Text = "Messages";
             BackColor = Color.FromArgb(248,249,250);
             Font = new Font("Segoe UI",9);
             Dock = DockStyle.Fill;
             _host = new Panel { Dock = DockStyle.Fill, BackColor = BackColor, Padding = new Padding(10) };
             Controls.Add(_host);
             Build();
        }

         private void Build()
         {
             _host.Controls.Clear();
             var splitter = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance =220 };
             _host.Controls.Add(splitter);
             _peers = new ListBox { Dock = DockStyle.Fill };
             splitter.Panel1.Controls.Add(_peers);
             _thread = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
             splitter.Panel2.Controls.Add(_thread);

             LoadPeers();
             _peers.SelectedIndexChanged += (s, e) => { var item = _peers.SelectedItem as PeerItem; if (item != null) { _peerId = item.Id; _peerType = item.Type; LoadThread(); } };
          }

         private readonly System.Collections.Generic.Dictionary<int,string> _userNames = new();

         private void LoadPeers()
         {
             _peers.Items.Clear();
             foreach (var conv in _messages.GetDirectConversations(_me.Id, "Seller"))
             {
                 var name = GetUserName(conv.PeerId);
                 _peers.Items.Add(new PeerItem { Id = conv.PeerId, Type = conv.PeerType, Text = $"{name} - {conv.LastAt:g} - {conv.Preview}" });
             }
         }

         private void LoadThread()
         {
             if (!_peerId.HasValue) return;
             _thread.Controls.Clear();
             int y =10;
             foreach (var m in _messages.GetDirect(_me.Id, "Seller", _peerId.Value, _peerType))
             {
                 var bubble = new Panel { Left =10, Top = y, Width = _thread.Width -40, Height =70, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
                 var who = new Label { Text = m.SenderType.Equals("User", StringComparison.OrdinalIgnoreCase) ? GetUserName(m.SenderId) : _me.CompanyName, Left =10, Top =8, AutoSize = true, Font = new Font("Segoe UI",8, FontStyle.Bold) };
                 var body = new Label { Text = string.IsNullOrEmpty(m.Body) ? "" : m.Body, Left =10, Top =26, Width = bubble.Width -20, Height =20 };
                 bubble.Controls.Add(who); bubble.Controls.Add(body);
                 _thread.Controls.Add(bubble);
                 y +=80;
             }
             var txt = new TextBox { Left =10, Top = _thread.Height -70, Width = _thread.Width -110, Height =60, Multiline = true, Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
             var send = new Button { Text = "Send", Left = txt.Right +10, Top = _thread.Height -70, Width =70, Height =60, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
             send.Click += (s, e) => { _messages.Add(new samp_01.Domain.Entities.Message { OrderId = null, SenderId = _me.Id, SenderType = "Seller", ReceiverId = _peerId, ReceiverType = _peerType, Body = string.IsNullOrWhiteSpace(txt.Text) ? null : txt.Text.Trim(), SentAt = DateTime.UtcNow }); txt.Clear(); LoadThread(); };
             _thread.Controls.Add(txt); _thread.Controls.Add(send);
         }

         private string GetUserName(int id)
         {
            if (_userNames.TryGetValue(id, out var n)) return n;
            var dto = _users.GetProfileById(id);
            n = dto?.Name ?? $"User #{id}";
            _userNames[id] = n;
            return n;
         }

         private class PeerItem
         {
             public int Id; public string Type = null!; public string Text = null!;
             public override string ToString() => Text;
         }
     }
}
