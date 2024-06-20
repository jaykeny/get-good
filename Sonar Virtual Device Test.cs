using System;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using NAudio.Wave.SampleProviders;

class Program
{
    static void Main(string[] args)
    {
        // Set up capture devices for Chat and Game
        var chatDevice = new WasapiLoopbackCapture(GetDevice("SteelSeries Sonar - Chat"));
        var gameDevice = new WasapiLoopbackCapture(GetDevice("SteelSeries Sonar - Gaming"));

        // Create buffers to store captured data
        var chatBuffer = new BufferedWaveProvider(chatDevice.WaveFormat) { BufferLength = 10 * chatDevice.WaveFormat.AverageBytesPerSecond, ReadFully = true }; // Adjust buffer length as needed
        var gameBuffer = new BufferedWaveProvider(gameDevice.WaveFormat) { BufferLength = 10 * gameDevice.WaveFormat.AverageBytesPerSecond, ReadFully = true }; // Adjust buffer length as needed

        // Set up mixing format (common format)
        var commonSampleRate = 44100;
        var commonChannels = 2;
        var commonFormat = WaveFormat.CreateIeeeFloatWaveFormat(commonSampleRate, commonChannels);

        // Create resampling providers to convert to common format
        var chatResampler = new MediaFoundationResampler(chatBuffer, commonFormat);
        var gameResampler = new MediaFoundationResampler(gameBuffer, commonFormat);

        // Create mixing sample provider with common format
        var mixer = new MixingSampleProvider(commonFormat);

        // Add resampled inputs to mixer
        mixer.AddMixerInput(chatResampler.ToSampleProvider());
        mixer.AddMixerInput(gameResampler.ToSampleProvider());

        // Set up playback device (the virtual device named "Headphones")
        var mixerOutputDevice = new WasapiOut(GetDevice("Headphones"), AudioClientShareMode.Shared, true, 1); // Example: 50 milliseconds buffer

        // Configure the output to use the mixed audio stream
        mixerOutputDevice.Init(new SampleToWaveProvider(mixer));

        // Handle captured data events
        chatDevice.DataAvailable += (s, e) =>
        {
            try
            {
                chatBuffer.AddSamples(e.Buffer, 0, e.BytesRecorded);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Chat buffer full: {ex.Message}");
            }
        };

        gameDevice.DataAvailable += (s, e) =>
        {
            try
            {
                gameBuffer.AddSamples(e.Buffer, 0, e.BytesRecorded);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Game buffer full: {ex.Message}");
            }
        };

        // Start capturing and playing
        chatDevice.StartRecording();
        gameDevice.StartRecording();
        mixerOutputDevice.Play();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

        // Stop capturing and playing
        chatDevice.StopRecording();
        gameDevice.StopRecording();
        mixerOutputDevice.Stop();
    }

    static MMDevice GetDevice(string friendlyName)
    {
        var enumerator = new MMDeviceEnumerator();
        foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
        {
            if (device.FriendlyName.Contains(friendlyName))
            {
                return device;
            }
        }
        throw new Exception($"Device with name {friendlyName} not found");
    }
}
