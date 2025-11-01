using System;
using System.Drawing;
using System.Windows.Forms;
using samp_01.Domain.DTO;

namespace samp_01.Forms.ServiceProvider
{
   
         public class ServiceProviderProfileForm : Form
         {
             public ServiceProviderProfileForm(SellerProfileDTO dto)
             {
                 Text = "Provider Profile";
                 ClientSize = new Size(480,360);
                 StartPosition = FormStartPosition.CenterParent;
                 Font = new Font("Segoe UI",9);

                 var lblTitle = new Label
                 {
                     Text = dto.CompanyName,
                     Font = new Font("Segoe UI",14, FontStyle.Bold),
                     Left =20, Top =12, AutoSize = true 
                 };
                 var pic = new PictureBox 
                 {   Left =20, Top =44, Width =120, Height =120,
                     BorderStyle = BorderStyle.FixedSingle, 
                     SizeMode = PictureBoxSizeMode.Zoom
                 };
                 if (!string.IsNullOrEmpty(dto.LogoPath) && System.IO.File.Exists(dto.LogoPath)) pic.Image = Image.FromFile(dto.LogoPath);

                 var lblEmail = new Label 
                 { 
                     Text = $"Email: {dto.Email}",
                     Left =160, Top =44, AutoSize = true
                 };
                 var lblPhone = new Label 
                 { Text = $"Phone: {dto.Phone}",
                     Left =160, Top =70, AutoSize = true 
                 };
                 var lblAddr = new Label 
                 { Text = $"Address: {dto.CompanyAddress}",
                     Left =160, Top =96, 
                     AutoSize = true, 
                     Width =280 
                 };
                 Controls.AddRange(new Control[] { lblTitle, pic, lblEmail, lblPhone, lblAddr });
             }
         }
}
