using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using NAudio.CoreAudioApi;

namespace Get_Good
{
    public static class Client
    {
        private static MMDevice chatDevice = null;
        private static MMDevice systemDevice = null;

        public static void StartClient()
        {
            TcpListener listener = null; // Renamed from 'client'
            Int32 port = 8089;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            try
            {
                listener = new TcpListener(localAddr, port);
                listener.Start();

                while (true)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient(); // Renamed to 'tcpClient'
                    HandleClient(tcpClient);
                }
            }
            catch (SocketException e)
            {
                MessageBox.Show($"SocketException: {e}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                listener?.Stop();
            }
        }


        private static void HandleClient(TcpClient client)
        {
            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            try
            {
                int i;

                // Loop to receive all the data sent by the client
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string
                    data = Encoding.ASCII.GetString(bytes, 0, i);

                    // Example: Parse received data and set audio levels accordingly
                    string[] parts = data.Split(',');
                    if (parts.Length >= 2 && Int32.TryParse(parts[0], out int voiceLevel) && Int32.TryParse(parts[1], out int systemLevel))
                    {
                        SetAudioLevels(voiceLevel, systemLevel);
                    }
                    else
                    {
                        // Handle invalid data if needed
                    }
                }
            }
            catch (Exception e)
            {
                // Handle exceptions
            }
            finally
            {
                // Close client connection
                client.Close();
            }
        }

        private static void SetAudioLevels(int voiceLevel, int systemLevel)
        {
            // Get the default audio endpoint device (speaker)
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            MMDeviceCollection devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            // Get the devices and store them, otherwise the loop causes lag
            if (chatDevice == null && systemDevice == null)
            {
                foreach (MMDevice device in devices)
                {
                    if (device.FriendlyName.Contains(Properties.Settings.Default.ChatInput))
                    {
                        chatDevice = device;
                    }
                    else if (device.FriendlyName.Contains(Properties.Settings.Default.VoiceInput))
                    {
                        systemDevice = device;
                    }
                }
            }

            if (chatDevice != null)
            {
                chatDevice.AudioEndpointVolume.MasterVolumeLevelScalar = voiceLevel / 100.0f;
            }

            if (systemDevice != null)
            {
                systemDevice.AudioEndpointVolume.MasterVolumeLevelScalar = systemLevel / 100.0f;
            }
        }
    }
}
