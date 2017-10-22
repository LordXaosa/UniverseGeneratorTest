using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Packets
{
    public class LoginPacket : IPacket
    {
        public bool IsAuth { get; set; }
        bool _isServer;
        public string Login;
        public string Password;
        public LoginPacket()
        {
            _isServer = true;
        }
        public LoginPacket(string login, string password)
        {
            Login = login;
            Password = password;
            _isServer = false;
        }
        public void ReadPacket(BinaryReader br)
        {
            if (!_isServer)
            {
                IsAuth = br.ReadBoolean();
            }
            else
            {
                Login = br.ReadString();
                Password = br.ReadString();
            }
        }

        public void WritePacket(BinaryWriter bw)
        {
            bw.Write((int)Entities.PacketsEnum.Login);
            if (!_isServer)
            {
                bw.Write(Login);
                bw.Write(Password);
            }
            else
            {
                bw.Write(IsAuth);
            }
        }
    }
}
