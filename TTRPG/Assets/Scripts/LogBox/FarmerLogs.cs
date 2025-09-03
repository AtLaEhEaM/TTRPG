using System;
using UnityEngine;

[Serializable]
public class FarmerLogs
{
    private string finalText;

    public string ConcutSring(int _case)
    {
        switch (_case)
        {
            // --- Positive Wheat ---
            case 0:
                return finalText = "Your <color=#00FF00>Farmers</color> harvest <color=#FFD700>Wheat</color><br>" +
                                   "<color=#A52A2A>+20 Wheat</color>";
            case 1:
                return finalText = "A <color=#00FF00>bountiful</color> <color=#FFD700>Wheat</color> harvest arrives<br>" +
                                   "<color=#A52A2A>+40 Wheat</color>";
            case 2:
                return finalText = "<color=#00FF00>Skilled farmers</color> improve yield of <color=#FFD700>Wheat</color><br>" +
                                   "<color=#A52A2A>+60 Wheat</color>";
            case 3:
                return finalText = "Fields of <color=#FFD700>Wheat</color> sway under gentle winds<br>" +
                                   "<color=#A52A2A>+80 Wheat</color>";

            // --- Positive Rice ---
            case 4:
                return finalText = "Your <color=#00FF00>Farmers</color> cultivate <color=#FFFFFF>Rice</color><br>" +
                                   "<color=#708090>+20 Rice</color>";
            case 5:
                return finalText = "<color=#00FF00>Abundant rain</color> blesses the <color=#FFFFFF>Rice</color> paddies<br>" +
                                   "<color=#708090>+40 Rice</color>";
            case 6:
                return finalText = "Your <color=#00FF00>villagers</color> feast on fresh <color=#FFFFFF>Rice</color><br>" +
                                   "<color=#708090>+60 Rice</color>";
            case 7:
                return finalText = "A <color=#00FF00>rich harvest</color> fills the granaries with <color=#FFFFFF>Rice</color><br>" +
                                   "<color=#708090>+80 Rice</color>";

            // --- Positive Meat ---
            case 8:
                return finalText = "<color=#00FF00>Hunters</color> return with fresh <color=#FF4500>Meat</color><br>" +
                                   "<color=#8B0000>+15 Meat</color>";
            case 9:
                return finalText = "A <color=#00FF00>great hunt</color> provides ample <color=#FF4500>Meat</color><br>" +
                                   "<color=#8B0000>+30 Meat</color>";
            case 10:
                return finalText = "Your <color=#00FF00>hunters</color> secure game for the tribe<br>" +
                                   "<color=#8B0000>+45 Meat</color>";
            case 11:
                return finalText = "The <color=#FF4500>Meat</color> stores grow full and hearty<br>" +
                                   "<color=#8B0000>+60 Meat</color>";

            // --- Positive Wood ---
            case 12:
                return finalText = "<color=#00FF00>Lumberjacks</color> gather strong <color=#8B4513>Wood</color><br>" +
                                   "<color=#DEB887>+25 Wood</color>";
            case 13:
                return finalText = "A <color=#00FF00>steady supply</color> of <color=#8B4513>Wood</color> arrives<br>" +
                                   "<color=#DEB887>+50 Wood</color>";
            case 14:
                return finalText = "Your <color=#00FF00>workers</color> fell mighty oaks<br>" +
                                   "<color=#DEB887>+75 Wood</color>";
            case 15:
                return finalText = "A <color=#00FF00>windfall</color> of timber fills the yard<br>" +
                                   "<color=#DEB887>+100 Wood</color>";

            // --- Negative Wheat ---
            case 16:
                return finalText = "<color=#FF0000>Disease</color> ravages the <color=#FFD700>Wheat</color> fields<br>" +
                                   "<color=#A52A2A>-20 Wheat</color>";
            case 17:
                return finalText = "<color=#FF0000>Locusts</color> devour your <color=#FFD700>Wheat</color><br>" +
                                   "<color=#A52A2A>-40 Wheat</color>";

            // --- Negative Rice ---
            case 18:
                return finalText = "Flooded paddies ruin your <color=#FFFFFF>Rice</color><br>" +
                                   "<color=#708090>-20 Rice</color>";
            case 19:
                return finalText = "<color=#FF0000>Pests</color> infest your <color=#FFFFFF>Rice</color> crop<br>" +
                                   "<color=#708090>-40 Rice</color>";

            // --- Negative Meat ---
            case 20:
                return finalText = "<color=#FF0000>Game migrates</color>, leaving hunters empty-handed<br>" +
                                   "<color=#8B0000>-15 Meat</color>";
            case 21:
                return finalText = "Spoiled <color=#FF4500>Meat</color> fouls the stores<br>" +
                                   "<color=#8B0000>-30 Meat</color>";

            // --- Negative Wood ---
            case 22:
                return finalText = "<color=#FF0000>Storms</color> scatter your stored <color=#8B4513>Wood</color><br>" +
                                   "<color=#DEB887>-25 Wood</color>";
            case 23:
                return finalText = "<color=#FF0000>Fire</color> consumes your <color=#8B4513>Wood</color> piles<br>" +
                                   "<color=#DEB887>-50 Wood</color>";
        }

        return null;
    }

}
