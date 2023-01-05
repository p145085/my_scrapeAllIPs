using System.Net.NetworkInformation;
using System.Net;

namespace my_scrapeAllIPs
{
    internal class Program
    {
        //There are two main types of IP addresses: IPv4 and IPv6. IPv4 addresses are 32-bit numbers that are typically expressed as four decimal numbers separated by dots (e.g., 192.168.1.1). There are approximately 4.3 billion IPv4 addresses, but many of these are reserved for special purposes or are not in use.

        //IPv6 addresses are 128-bit numbers that are expressed as eight groups of four hexadecimal digits separated by colons(e.g., 2001:0db8:85a3:0000:0000:8a2e:0370:7334). There are approximately 3.4 x 10^38 IPv6 addresses, which is significantly more than the number of IPv4 addresses.

        //In general, only a small fraction of IP addresses are used to host websites, as most IP addresses are used for other purposes such as connecting devices to the internet or private networks.

        static void Main(string[] args)
        {
            // Configure the list of IP addresses to scrape
            List<string> usedIPs = new List<string>();
            List<string> nonPingable = new List<string>();
            int first = 0;
            int second = 0;
            int third = 0;
            int fourth = 0;
            // Set the directory where the text files will be saved
            string saveDirectory = "D:\\emiltempscraping";
            Random random = new Random();

            string GetRandomIPAddress()
            {
                int first = random.Next(0, 255);
                int second = random.Next(0, 255);
                int third = random.Next(0, 255);
                int fourth = random.Next(0, 255);
                return $"{first}.{second}.{third}.{fourth}";
            }
            // Set a variable to use for our operations.
            string current = GetRandomIPAddress();
            Console.WriteLine(current);

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
                                string url = "http://" + ip + ":80";
                                PingReply reply = ping.Send(ip, 5000);
                                if (reply.Status == IPStatus.Success)
                                {
                                    // IP address is pingable, so download the data
                                    try
                                    {
                                        string theData = client.DownloadString("http://" + ip + ":80");
                                        return theData;
                                    }
                                    catch (WebException ex)
                                    {
                                        if (ex.Status == WebExceptionStatus.Timeout)
                                        {
                                            Console.WriteLine("Connection timed out!");
                                            return null;
                                        }
                                        else
                                        {
                                            // Some other error occurred
                                            Console.WriteLine(ex);
                                            return null;
                                        }
                                    }
                                }
                                else
                                {
                                    // IP address is not pingable
                                    nonPingable.Add(ip);
                                    return null;
                                }
                            } else
                            {
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

            string dataFound = GetWebPageData(current);

            if (dataFound != null)
            {
                SaveToTextFile(dataFound, saveDirectory, current);
            }

            // Don't add IP to list until we're done.
            usedIPs.Add(current);

            //foreach (string ip in ips_list)
            //{
            //    // Retrieve the web page data
            //    string data = GetWebPageData(ip);
            //    // Save the data to a text file
            //    if (data != null)
            //    {
            //        SaveToTextFile(data, saveDirectory, ip);
            //    }
            //}
        }
    }
}