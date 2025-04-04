using System;
using System.Drawing;
using System.IO;
using System.Media;
using System.Speech.Synthesis;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

class AsciiArtDisplay
{
    public void DisplayImageAsAsciiArt()
    {
        string fullLocation = AppDomain.CurrentDomain.BaseDirectory;
        string newLocation = fullLocation.Replace("bin\\Debug\\", "");
        string fullPath = Path.Combine(newLocation, "eddie.jpg");

        using (Bitmap image = new Bitmap(fullPath))
        {
            Bitmap resizedImage = new Bitmap(image, new Size(80, 60));

            for (int height = 0; height < resizedImage.Height; height++)
            {
                for (int width = 0; width < resizedImage.Width; width++)
                {
                    Color pixelColor = resizedImage.GetPixel(width, height);
                    int gray = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                    char asciiChar = gray > 200 ? '.' : gray > 150 ? '*' : gray > 100 ? '0' : gray > 50 ? '#' : '@';
                    Console.Write(asciiChar);
                }
                Console.WriteLine();
            }
        }
    }
}

class ChatKnowledge
{
    public Dictionary<string, string> Topics = new Dictionary<string, string>
    {
        {"phishing", "Phishing is an attack where hackers trick users into revealing sensitive information. Have you ever received a suspicious email? (yes/no)"},
        {"password", "Use at least 12 characters, mixing letters, numbers, and symbols for a strong password! Do you use a password manager? (yes/no)"},
        {"malware", "Malware is malicious software that can harm your device or steal data. Would you like to know how to remove malware? (yes/no)"},
        {"safe online", "Always use strong passwords, enable 2FA, avoid suspicious links, and keep software updated. Do you want a step-by-step guide on online safety? (yes/no)"},
        {"firewall", "A firewall acts as a barrier between your computer and the internet. Want to know how to set one up? (yes/no)"},
        {"ransomware", "Ransomware locks your files and demands payment. Want tips on prevention and backup strategies? (yes/no)"},
        {"apartheid", "Apartheid was a system of institutionalized racial segregation in South Africa from 1948 until the early 1990s."},
        {"mandela", "Nelson Mandela was South Africa’s first Black president and an iconic anti-apartheid revolutionary."},
        {"bafana bafana", "Bafana Bafana is South Africa’s national football team. Want to know more about their achievements?"},
        {"loadshedding", "Loadshedding is the planned interruption of electricity supply to manage demand and prevent total blackouts."},
        {"who is jarvis", "JARVIS is your helpful AI assistant who knows a little bit of everything."},
        {"who's a genius", "You are! Obviously! 😉"},
        {"howzit", "I'm chillin', thanks for asking! How can I help today?"}
    };

    public string[] SmallTalkQuestions = { "how are you", "how’s it going", "what’s up", "how are you doing", "what about you", "howzit", "who is jarvis", "who's a genius" };

    public string[] FollowUps = { "yes", "no" };
}

class JARVIS
{
    private string _username;
    private SpeechSynthesizer _synth;
    private AsciiArtDisplay _asciiArtDisplay;
    private ChatKnowledge _knowledge;
    private string lastTopic = "";

    public JARVIS()
    {
        _username = "Stranger";
        _synth = new SpeechSynthesizer();
        _asciiArtDisplay = new AsciiArtDisplay();
        _knowledge = new ChatKnowledge();
    }

    public void Start()
    {
        PlayAudioIntro();
        _asciiArtDisplay.DisplayImageAsAsciiArt();
        DisplayBio();
        GreetUser();
        ShowInstructions();
        StartChatLoop();
    }

    private void DisplayBio()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        string bio = "I am JARVIS, your AI assistant. I specialize in cybersecurity and South African general knowledge.";
        Console.WriteLine(bio);
        Console.ResetColor();
        Speak(bio);
    }

    private void GreetUser()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("Hey there! What's your name? ");
        Console.ResetColor();

        string inputName = Console.ReadLine()?.Trim();
        if (!string.IsNullOrWhiteSpace(inputName))
            _username = inputName;

        string greeting = $"\nHi, {_username}! You're now chatting with JARVIS.";
        Console.WriteLine(greeting);
        Speak(greeting);
    }

    private void ShowInstructions()
    {
        Console.WriteLine("\nHere's what you can ask about:");
        Console.WriteLine("- Cybersecurity: phishing, passwords, malware, ransomware, firewall, safe online");
        Console.WriteLine("- South African Knowledge: apartheid, Mandela, Bafana Bafana, loadshedding");
        Console.WriteLine("- Small talk: How are you, Howzit, Who is Jarvis, how's it going, who's a genius (I'll say 'you are!')");
    }

    private void StartChatLoop()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\nAsk me something (or type 'bye' to exit): ");
            Console.ResetColor();

            string userInput = Console.ReadLine()?.ToLower().Trim();
            if (userInput == "bye") break;

            string reply = GenerateReply(userInput);
            SimulateTyping(reply);
            Speak(reply);
        }

        Console.WriteLine("\nTake care! Stay cyber-safe.");
        Speak("Take care! Stay cyber-safe.");
    }

    private string GenerateReply(string input)
    {
        if (_knowledge.FollowUps.Contains(input) && !string.IsNullOrEmpty(lastTopic))
            return HandleFollowUp(input);

        if (_knowledge.SmallTalkQuestions.Any(talk => input.Contains(talk)))
        {
            return "I'm great, thanks for asking! Always ready to help. How can I assist you today?";
        }

        foreach (var topic in _knowledge.Topics)
        {
            if (input == topic.Key ||
                input == "what is " + topic.Key ||
                input == "tell me about " + topic.Key)
            {
                lastTopic = topic.Key;
                return topic.Value;
            }
        }

        return "Hmm, I didn't quite catch that. Please ask a clear question from the list or say 'bye' to exit.";
    }

    private string HandleFollowUp(string response)
    {
        switch (lastTopic)
        {
            case "phishing":
                return response == "yes" ? "Be careful! Never click on suspicious links or enter personal info in emails. Would you like to learn how to report phishing attempts? (yes/no)" : "That's great! But always stay alert. Want to know how to recognize phishing scams? (yes/no)";
            case "password":
                return response == "yes" ? "That's great! Password managers make things safer and easier." : "You should try one out! They help manage complex passwords securely.";
            case "malware":
                return response == "yes" ? "Use antivirus tools like Malwarebytes or Windows Defender. Need help picking one? (yes/no)" : "Cool. Just remember to update your system and avoid sketchy links.";
            case "safe online":
                return response == "yes" ? "Step 1: Use long passwords. Step 2: Enable 2FA. Step 3: Don’t overshare online. Want more?" : "Alright. Let me know if you'd like a refresher later.";
            default:
                return "Alright! Feel free to ask me anything else from the list.";
        }
    }

    private void SimulateTyping(string message)
    {
        foreach (char letter in message)
        {
            Console.Write(letter);
            Thread.Sleep(25);
        }
        Console.WriteLine();
    }

    private void Speak(string text)
    {
        try
        {
            _synth.Speak(text);
        }
        catch (Exception err)
        {
            Console.WriteLine("[Speech error: " + err.Message + "]");
        }
    }

    private void PlayAudioIntro()
    {
        try
        {
            if (System.IO.File.Exists("greeting.wav"))
            {
                SoundPlayer audio = new SoundPlayer("greeting.wav");
                audio.PlaySync();
            }
            else
            {
                Console.WriteLine("[Audio file not found: greeting.wav]");
            }
        }
        catch (Exception err)
        {
            Console.WriteLine("\n[Oops! Couldn't play the audio: " + err.Message + "]");
        }
    }
}

class Program
{
    static void Main()
    {
        JARVIS jarvis = new JARVIS();
        jarvis.Start();
    }
}
