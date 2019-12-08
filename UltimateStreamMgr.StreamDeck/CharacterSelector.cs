using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BarRaider.SdTools;
using NLog.Layouts;
using UltimateStreamMgr.Api.Entities;
using UltimateStreamMgr.Api.Messages;

namespace UltimateStreamMgr.StreamDeck
{
    public static class CharacterSelector
    {
        private static List<ProgrammableButton> buttons = new List<ProgrammableButton>();

        private static int buttonCount = 0;

        private static List<CharacterInfo> characters = new List<CharacterInfo>();

        private static bool keyReady = false;
        private static bool characterReady = false;


        public static void Initialize()
        {
            buttons.Clear();
            characters.Clear();
            buttonCount = 0;
            keyReady = characterReady = false;
            Task.Run(() =>
            {
                USM.OnMessageReceived += (m) =>
                {
                    if (m is CharacterListMessage message)
                    {
                        characters = message.Characters;
                        characterReady = true;

                        while(!keyReady)
                            Thread.Sleep(10);
                        InitializeGrid();
                    }

                };
                USM.Send(new GetCharacterListMessage());
            }).Wait();
        }

        public static void AddKey(ProgrammableButton button)
        {
            if (buttons.Count == 0) // it's the first button we will use its data to know when we have everything
            {
                switch (button.ButtonInfo.DeviceInfo.Devices.First().Type)
                {
                    case StreamDeckDeviceType.StreamDeckClassic:
                        buttonCount = 15;
                        break;
                    case StreamDeckDeviceType.StreamDeckXL:
                        buttonCount = 32;
                        break;
                    case StreamDeckDeviceType.StreamDeckMini:
                        buttonCount = 6;
                        break;
                    case StreamDeckDeviceType.StreamDeckMobile: // Unsupported
                    default: // Unsupported
                        break;

                }

                buttons.Add(button);
            }
            else if (buttons.Count == buttonCount - 1) // We have everyone we can start !
            {
                buttons.Add(button);

                keyReady = true;
            }
            else
            {
                buttons.Add(button);
            }
        }

        private static void InitializeGrid()
        {
            ChangeGridPage(0);
        }

        private static void ChangeGridPage(int page)
        {
            int minPage = 0;
            int maxPage = characters.Count / 10;

            foreach (var button in buttons)
            {
                KeyCoordinates coordinates = button.ButtonInfo.Coordinates;
                if (coordinates.Row == 0)
                {
                    switch (coordinates.Column)
                    {
                        case 0: // Return button
                            button.SetImage(@"C:\Users\Benjamin\Pictures\padchance2.png");
                            button.OnClick = () => { button.Connection.SwitchProfileAsync(""); };
                            break;
                        case 1: // Prev button
                            button.SetImage(@"D:\share\Pictures\paluchibi.png");
                            if (page > minPage)
                                button.OnClick = () => { ChangeGridPage(page - 1); };
                            break;
                        case 2: // Indicator
                            button.Connection.SetTitleAsync($"{page + 1}/{maxPage}");
                            break;
                        case 3: // Next button
                            button.SetImage(@"D:\share\Pictures\lachance.png");
                            if (page < maxPage)
                                button.OnClick = () => { ChangeGridPage(page + 1); };
                            break;
                        case 4: // Unused button
                            break;
                    }

                }
                else
                {
                    int characterPosition = page * 10 + coordinates.Column + (coordinates.Row - 1) * 5;
                    if (characterPosition >= characters.Count)
                    {
                        button.ResetImage();
                    }
                    else
                    {
                        var pickedCharacter = characters[characterPosition];
                        button.SetImage(pickedCharacter.IconPath);
                        if (pickedCharacter.Alts.Count > 0)
                        {
                            button.OnClick = () => { LoadAltGrid(pickedCharacter, page, 0); };
                        }
                        else
                        {
                            button.OnClick = () =>
                            {
                                USM.Send(new ChangeCharacterMessage{ CharacterName = pickedCharacter.Name, PlayerId = 1});
                                button.Connection.SwitchProfileAsync("");
                            };
                        }
                    }
                }
            }

        }

        private static void LoadAltGrid(CharacterInfo pickedCharacter, int sourcePage, int altPage)
        {
            int minPage = 0;
            int maxPage = characters.Count / 10;

            foreach (var button in buttons)
            {
                KeyCoordinates coordinates = button.ButtonInfo.Coordinates;
                if (coordinates.Row == 0)
                {
                    switch (coordinates.Column)
                    {
                        case 0: // Return button
                            button.SetImage(@"C:\Users\Benjamin\Pictures\padchance2.png");
                            button.OnClick = () => { ChangeGridPage(sourcePage); };
                            break;
                        case 1: // Prev button
                            button.SetImage(@"D:\share\Pictures\paluchibi.png");
                            if (altPage > minPage)
                                button.OnClick = () => { LoadAltGrid(pickedCharacter, sourcePage, altPage - 1); };
                            break;
                        case 2: // Indicator
                            button.Connection.SetTitleAsync($"{altPage + 1}/{maxPage}");
                            break;
                        case 3: // Next button
                            button.SetImage(@"D:\share\Pictures\lachance.png");
                            if (altPage < maxPage)
                                button.OnClick = () => { LoadAltGrid(pickedCharacter, sourcePage, altPage + 1); };
                            break;
                        case 4: // Unused button
                            break;
                    }

                }
                else
                {
                    int altCharacterPosition = altPage * 10 + coordinates.Column + (coordinates.Row - 1) * 5;
                    if (altCharacterPosition >= pickedCharacter.Alts.Count)
                    {
                        button.ResetImage();
                    }
                    else
                    {
                        var pickedAlt = pickedCharacter.Alts[altCharacterPosition];
                        button.SetImage(pickedAlt.IconPath);
                        button.OnClick = () =>
                        {
                            USM.Send(new ChangeCharacterMessage { CharacterName = pickedAlt.Name, PlayerId = 1 });
                            button.Connection.SwitchProfileAsync("");
                        };
                    }
                }
            }
        }
    }
}
