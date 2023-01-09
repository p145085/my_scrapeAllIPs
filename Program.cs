using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace my_scrapeAllIPs
{
    internal class Program
    {
        //There are two main types of IP addresses: IPv4 and IPv6. IPv4 addresses are 32-bit numbers that are typically expressed as four decimal numbers separated by dots (e.g., 192.168.1.1). There are approximately 4.3 billion IPv4 addresses, but many of these are reserved for special purposes or are not in use.

        //IPv6 addresses are 128-bit numbers that are expressed as eight groups of four hexadecimal digits separated by colons(e.g., 2001:0db8:85a3:0000:0000:8a2e:0370:7334). There are approximately 3.4 x 10^38 IPv6 addresses, which is significantly more than the number of IPv4 addresses.

        //In general, only a small fraction of IP addresses are used to host websites, as most IP addresses are used for other purposes such as connecting devices to the internet or private networks.

        static void Main(string[] args)
        {
            // ** -- Declaration of variables -- ** //
            List<string> usedIPs = new List<string>(); // To be filled with IP's checked.
            List<string> nonPingable = new List<string>(); // Save the IP's that aren't pingable.
            Random random = new Random();

            // ** -- Configuration -- ** //
            // Set the directory where the text files will be saved.
            string saveDirectory = "D:\\emiltempscraping";

            string GetRandomIPAddress()
            {
                int first = random.Next(0, 255);
                int second = random.Next(0, 255);
                int third = random.Next(0, 255);
                int fourth = random.Next(0, 255);
                return $"{first}.{second}.{third}.{fourth}";
            }

            // ** -- Method Declaration(s) -- ** //
            // Method to check if a submitted IP exists in our 'used' List.
            bool IsIPUsed(string ip)
            {
                return usedIPs.Contains(ip);
            }

            static void SaveToTextFile(string data, string saveDirectory, string ip)
            {
                // Create the file path
                string filePath = Path.Combine(saveDirectory, ip + ".txt");

                // Write the data to the file
                File.WriteAllText(filePath, data);
            }

            string GetWebPageData(string ip)
            {
                using (WebClient client = new WebClient())
                {
                    using (Ping ping = new Ping())
                    {
                        try
                        {
                            if (!IsIPUsed(ip)) // Is the IP already used?
                            {
                                string url = "http://" + ip + ":80"; // Define the address.
                                PingReply reply = ping.Send(ip, 5000); // Try to send a 'Ping'.
                                if (reply.Status == IPStatus.Success) 
                                {
                                    // IP address is pingable, so download the data.
                                    try
                                    {
                                        string theData = client.DownloadString(url);
                                        return theData;
                                    }
                                    catch (WebException ex)
                                    {
                                        if (ex.Status == WebExceptionStatus.Timeout)
                                        {
                                            // 'Ping' took too long to respond.
                                            Console.WriteLine("Connection timed out!");
                                            return null;
                                        }
                                        else
                                        {
                                            // Some other error occurred.
                                            Console.WriteLine(ex);
                                            return null;
                                        }
                                    }
                                }
                                else
                                {
                                    // IP address is not pingable.
                                    nonPingable.Add(ip);
                                    return null;
                                }
                            }
                            else 
                            {
                                // The IP was in the 'usedIPs' list.
                                return null;
                            }
                        }
                        catch (PingException ex)
                        {
                            return ex.Message;
                        }
                        catch (WebException wex)
                        {
                            return wex.Message;
                        }
                    }
                }
            }

            static bool IsPortOpen(string ip, int port)
            {
                using (TcpClient client = new TcpClient())
                {
                    try
                    {
                        client.Connect(ip, port);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            // ** -- Program / Application / Running process -- ** //
            // Set a variable to use for our operations.
            string current = GetRandomIPAddress();
            Console.WriteLine(current);

            // We want to get to know the host, what's happening there? Let's scan some ports.
            // Ports range from 0 to 65536.
            int[] ports = { 21, 22, 80, 443 }; // Can define a selection of popular ports.
            for (int i = 0; i <= 65536; i++) { } // Can also search all of the ports.
            foreach (int port in ports)
            {
                if (IsPortOpen(current, port))
                {
                    if (port == 21)
                    {
                        Console.WriteLine($"Port {port} is commonly used for FTP.");
                    }
                    else if (port == 22)
                    {
                        Console.WriteLine($"Port {port} is commonly used for SSH.");
                    }
                    Console.WriteLine($"Port {port} is open.");
                }
                else
                {
                    Console.WriteLine($"Port {port} is closed.");
                }
            }

            // Retrieve the data at the current address.
            string data = GetWebPageData(current);
            if (data != null)
            {
                // Some data was found, save it to a text file.
                SaveToTextFile(data, saveDirectory, current);
            }

            // Final statement.
            usedIPs.Add(current);
        }
    }
}