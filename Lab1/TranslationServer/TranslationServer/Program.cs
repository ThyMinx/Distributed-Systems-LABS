using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

using TranslationInterface;

namespace TranslationServer
{
    class Program
    {
        static void Main(string[] args)
        {

            TcpChannel channel = new TcpChannel(5000); ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Translator), "Translate", WellKnownObjectMode.SingleCall);
            Console.WriteLine("Press the enter key to exit...");
            System.Console.ReadLine();

        }
    }

    public class Translator : MarshalByRefObject, ITranslation
    {
        string ITranslation.Translate(string EnglishString)
        {
            string[] words = EnglishString.Split(' ');
            string result = "";
            foreach (string word in words)
            {
                result += word.Substring(1);
                result += word.Substring(0, 1) + "ay ";
            }
            return result;
        }

        string ITranslation.GetName()
        {
            return "James Cairns";
        }

        string ITranslation.GetStudentId()
        {
            return "201601620";
        }
    }
}
