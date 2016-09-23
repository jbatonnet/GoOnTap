using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Math;

public class PokemonInfo
{
    public int Id { get; }

    public string EnglishName { get; }
    public string FrenchName { get; }

    public int HP { get; }
    public int Attack { get; }
    public int Defense { get; }
    public int SpecialAttack { get; }
    public int SpecialDefense { get; }
    public int Speed { get; }

    public int BaseStamina { get; }
    public int BaseAttack { get; }
    public int BaseDefense { get; }

    public PokemonInfo(int id, string englishName, string frenchName, int hp, int attack, int defense, int specialAttack, int specialDefense, int speed)
    {
        Id = id;

        EnglishName = englishName;
        FrenchName = frenchName;

        HP = hp;
        Attack = attack;
        Defense = defense;
        SpecialAttack = specialAttack;
        SpecialDefense = specialDefense;
        Speed = speed;

        BaseAttack = (int)(2 * Abs(Sqrt(Attack) * Sqrt(SpecialAttack) + Sqrt(Speed)));
        BaseDefense = (int)(2 * Abs(Sqrt(Defense) * Sqrt(SpecialDefense) + Sqrt(Speed)));
        BaseStamina = HP * 2;
    }
    public PokemonInfo(int id, string englishName, string frenchName, int baseAttack, int baseDefense, int baseStamina)
    {
        Id = id;

        EnglishName = englishName;
        FrenchName = frenchName;
        
        BaseAttack = baseAttack;
        BaseDefense = baseDefense;
        BaseStamina = baseStamina;
    }

    public int GetCP(double level, int attack, int defense, int stamina) => (int)(((BaseAttack + attack) * Sqrt(BaseDefense + defense) * Sqrt(BaseStamina + stamina) * Pow(Constants.CPMultipliers[level], 2)) / 10);
    public int GetMinimumCP(double level) => GetCP(level, 0, 0, 0);
    public int GetMaximumCP(double level) => GetCP(level, 15, 15, 15);

    public int GetHP(double level, int stamina) => (int)((BaseStamina + stamina) * Constants.CPMultipliers[level]);
    public int GetMinimumHP(double level) => GetHP(level, 0);
    public int GetMaximumHP(double level) => GetHP(level, 15);
}

public partial class Constants
{
    public static Dictionary<double, double> CPMultipliers { get; } = new Dictionary<double, double>()
    {
        [1.0] = 0.094,
        [1.5] = 0.135137432,
        [2.0] = 0.16639787,
        [2.5] = 0.192650919,
        [3.0] = 0.21573247,
        [3.5] = 0.236572661,
        [4.0] = 0.25572005,
        [4.5] = 0.273530381,
        [5.0] = 0.29024988,
        [5.5] = 0.306057377,
        [6.0] = 0.3210876,
        [6.5] = 0.335445036,
        [7.0] = 0.34921268,
        [7.5] = 0.362457751,
        [8.0] = 0.37523559,
        [8.5] = 0.387592406,
        [9.0] = 0.39956728,
        [9.5] = 0.411193551,
        [10.0] = 0.42250001,
        [10.5] = 0.432926419,
        [11.0] = 0.44310755,
        [11.5] = 0.4530599578,
        [12.0] = 0.46279839,
        [12.5] = 0.472336083,
        [13.0] = 0.48168495,
        [13.5] = 0.4908558,
        [14.0] = 0.49985844,
        [14.5] = 0.508701765,
        [15.0] = 0.51739395,
        [15.5] = 0.525942511,
        [16.0] = 0.53435433,
        [16.5] = 0.542635767,
        [17.0] = 0.55079269,
        [17.5] = 0.558830576,
        [18.0] = 0.56675452,
        [18.5] = 0.574569153,
        [19.0] = 0.58227891,
        [19.5] = 0.589887917,
        [20.0] = 0.59740001,
        [20.5] = 0.604818814,
        [21.0] = 0.61215729,
        [21.5] = 0.619399365,
        [22.0] = 0.62656713,
        [22.5] = 0.633644533,
        [23.0] = 0.64065295,
        [23.5] = 0.647576426,
        [24.0] = 0.65443563,
        [24.5] = 0.661214806,
        [25.0] = 0.667934,
        [25.5] = 0.674577537,
        [26.0] = 0.68116492,
        [26.5] = 0.687680648,
        [27.0] = 0.69414365,
        [27.5] = 0.700538673,
        [28.0] = 0.70688421,
        [28.5] = 0.713164996,
        [29.0] = 0.71939909,
        [29.5] = 0.725571552,
        [30.0] = 0.7317,
        [30.5] = 0.734741009,
        [31.0] = 0.73776948,
        [31.5] = 0.740785574,
        [32.0] = 0.74378943,
        [32.5] = 0.746781211,
        [33.0] = 0.74976104,
        [33.5] = 0.752729087,
        [34.0] = 0.75568551,
        [34.5] = 0.758630378,
        [35.0] = 0.76156384,
        [35.5] = 0.764486065,
        [36.0] = 0.76739717,
        [36.5] = 0.770297266,
        [37.0] = 0.7731865,
        [37.5] = 0.776064962,
        [38.0] = 0.77893275,
        [38.5] = 0.781790055,
        [39.0] = 0.78463697,
        [39.5] = 0.787473578,
        [40.0] = 0.79030001
    };

    public static PokemonInfo[] Pokemons { get; } = new PokemonInfo[]
    {
        new PokemonInfo(1, "Bulbasaur", "Bulbizarre", 126, 126, 90), // 45, 49, 49, 65, 65, 45
        new PokemonInfo(2, "Ivysaur", "Herbizarre", 156, 158, 120), // 60, 62, 63, 80, 80, 60
        new PokemonInfo(3, "Venusaur", "Florizarre", 198, 200, 160), // 80, 82, 83, 100, 100, 80
        new PokemonInfo(4, "Charmander", "Salamèche", 128, 108, 78), // 39, 52, 43, 60, 50, 65
        new PokemonInfo(5, "Charmeleon", "Reptincel", 160, 140, 116), // 58, 64, 58, 80, 65, 80
        new PokemonInfo(6, "Charizard", "Dracaufeu", 212, 182, 156), // 78, 84, 78, 109, 85, 100
        new PokemonInfo(7, "Squirtle", "Carapuce", 112, 142, 88), // 44, 48, 65, 50, 64, 43
        new PokemonInfo(8, "Wartortle", "Carabaffe", 144, 176, 118), // 59, 63, 80, 65, 80, 58
        new PokemonInfo(9, "Blastoise", "Tortank", 186, 222, 158), // 79, 83, 100, 85, 105, 78
        new PokemonInfo(10, "Caterpie", "Chenipan", 62, 66, 90), // 45, 30, 35, 20, 20, 45
        new PokemonInfo(11, "Metapod", "Chrysacier", 56, 86, 100), // 50, 20, 55, 25, 25, 30
        new PokemonInfo(12, "Butterfree", "Papilusion", 144, 144, 120), // 60, 45, 50, 90, 80, 70
        new PokemonInfo(13, "Weedle", "Aspicot", 68, 64, 80), // 40, 35, 30, 20, 20, 50
        new PokemonInfo(14, "Kakuna", "Conconfort", 62, 82, 90), // 45, 25, 50, 25, 25, 35
        new PokemonInfo(15, "Beedrill", "Dardargnan", 144, 130, 130), // 65, 90, 40, 45, 80, 75
        new PokemonInfo(16, "Pidgey", "Roucool", 94, 90, 80), // 40, 45, 40, 35, 35, 56
        new PokemonInfo(17, "Pidgeotto", "Roucoups", 126, 122, 126), // 63, 60, 55, 50, 50, 71
        new PokemonInfo(18, "Pidgeot", "Roucarnage", 170, 166, 166), // 83, 80, 75, 70, 70, 101
        new PokemonInfo(19, "Rattata", "Rattata", 92, 86, 60), // 30, 56, 35, 25, 35, 72
        new PokemonInfo(20, "Raticate", "Rattatac", 146, 150, 110), // 55, 81, 60, 50, 70, 97
        new PokemonInfo(21, "Spearow", "Piafabec", 102, 78, 80), // 40, 60, 30, 31, 31, 70
        new PokemonInfo(22, "Fearow", "Rapasdepic", 168, 146, 130), // 65, 90, 65, 61, 61, 100
        new PokemonInfo(23, "Ekans", "Abo", 112, 112, 70), // 35, 60, 44, 40, 54, 55
        new PokemonInfo(24, "Arbok", "Arbok", 166, 166, 120), // 60, 85, 69, 65, 79, 80
        new PokemonInfo(25, "Pikachu", "Pikachu", 124, 108, 70), // 35, 55, 40, 50, 50, 90
        new PokemonInfo(26, "Raichu", "Raichu", 200, 154, 120), // 60, 90, 55, 90, 80, 110
        new PokemonInfo(27, "Sandshrew", "Sabelette", 90, 114, 100), // 50, 75, 85, 20, 30, 40
        new PokemonInfo(28, "Sandslash", "Sablaireau", 150, 172, 150), // 75, 100, 110, 45, 55, 65
        new PokemonInfo(29, "Nidoran♀", "Nidoran♀", 100, 104, 110), // 55, 47, 52, 40, 40, 41
        new PokemonInfo(30, "Nidorina", "Nidorina", 132, 136, 140), // 70, 62, 67, 55, 55, 56
        new PokemonInfo(31, "Nidoqueen", "Nidoqueen", 184, 190, 180), // 90, 92, 87, 75, 85, 76
        new PokemonInfo(32, "Nidoran♂", "Nidoran♂", 110, 94, 92), // 46, 57, 40, 40, 40, 50
        new PokemonInfo(33, "Nidorino", "Nidorino", 142, 128, 122), // 61, 72, 57, 55, 55, 65
        new PokemonInfo(34, "Nidoking", "Nidoking", 204, 170, 162), // 81, 102, 77, 85, 75, 85
        new PokemonInfo(35, "Clefairy", "Mélofée", 116, 124, 140), // 70, 45, 48, 60, 65, 35
        new PokemonInfo(36, "Clefable", "Mélodelfe", 178, 178, 190), // 95, 70, 73, 95, 90, 60
        new PokemonInfo(37, "Vulpix", "Goupix", 106, 118, 76), // 38, 41, 40, 50, 65, 65
        new PokemonInfo(38, "Ninetales", "Feunard", 176, 194, 146), // 73, 76, 75, 81, 100, 100
        new PokemonInfo(39, "Jigglypuff", "Rondoudou", 98, 54, 230), // 115, 45, 20, 45, 25, 20
        new PokemonInfo(40, "Wigglytuff", "Grodoudou", 168, 108, 280), // 140, 70, 45, 85, 50, 45
        new PokemonInfo(41, "Zubat", "Nosferapti", 88, 90, 80), // 40, 45, 35, 30, 40, 55
        new PokemonInfo(42, "Golbat", "Nosferalto", 164, 164, 150), // 75, 80, 70, 65, 75, 90
        new PokemonInfo(43, "Oddish", "Mystherbe", 134, 130, 90), // 45, 50, 55, 75, 65, 30
        new PokemonInfo(44, "Gloom", "Ortide", 162, 158, 120), // 60, 65, 70, 85, 75, 40
        new PokemonInfo(45, "Vileplume", "Rafflesia", 202, 190, 150), // 75, 80, 85, 110, 90, 50
        new PokemonInfo(46, "Paras", "Paras", 122, 120, 70), // 35, 70, 55, 45, 55, 25
        new PokemonInfo(47, "Parasect", "Parasect", 162, 170, 120), // 60, 95, 80, 60, 80, 30
        new PokemonInfo(48, "Venonat", "Mimitoss", 108, 118, 120), // 60, 55, 50, 40, 55, 45
        new PokemonInfo(49, "Venomoth", "Aéromite", 172, 154, 140), // 70, 65, 60, 90, 75, 90
        new PokemonInfo(50, "Diglett", "Taupiqueur", 108, 86, 20), // 10, 55, 25, 35, 45, 95
        new PokemonInfo(51, "Dugtrio", "Triopikeur", 148, 140, 70), // 35, 80, 50, 50, 70, 120
        new PokemonInfo(52, "Meowth", "Miaouss", 104, 94, 80), // 40, 45, 35, 40, 40, 90
        new PokemonInfo(53, "Persian", "Persian", 156, 146, 130), // 65, 70, 60, 65, 65, 115
        new PokemonInfo(54, "Psyduck", "Psykokwak", 132, 112, 100), // 50, 52, 48, 65, 50, 55
        new PokemonInfo(55, "Golduck", "Akwakwak", 194, 176, 160), // 80, 82, 78, 95, 80, 85
        new PokemonInfo(56, "Mankey", "Férosinge", 122, 96, 80), // 40, 80, 35, 35, 45, 70
        new PokemonInfo(57, "Primeape", "Colosinge", 178, 150, 130), // 65, 105, 60, 60, 70, 95
        new PokemonInfo(58, "Growlithe", "Caninos", 156, 110, 110), // 55, 70, 45, 70, 50, 60
        new PokemonInfo(59, "Arcanine", "Arcanin", 230, 180, 180), // 90, 110, 80, 100, 80, 95
        new PokemonInfo(60, "Poliwag", "Ptitard", 108, 98, 80), // 40, 50, 40, 40, 40, 90
        new PokemonInfo(61, "Poliwhirl", "Têtarte", 132, 132, 130), // 65, 65, 65, 50, 50, 90
        new PokemonInfo(62, "Poliwrath", "Tartard", 180, 202, 180), // 90, 95, 95, 70, 90, 70
        new PokemonInfo(63, "Abra", "Abra", 110, 76, 50), // 25, 20, 15, 105, 55, 90
        new PokemonInfo(64, "Kadabra", "Kadabra", 150, 112, 80), // 40, 35, 30, 120, 70, 105
        new PokemonInfo(65, "Alakazam", "Alakazam", 186, 152, 110), // 55, 50, 45, 135, 95, 120
        new PokemonInfo(66, "Machop", "Machoc", 118, 96, 140), // 70, 80, 50, 35, 35, 35
        new PokemonInfo(67, "Machoke", "Machopeur", 154, 144, 160), // 80, 100, 70, 50, 60, 45
        new PokemonInfo(68, "Machamp", "Mackogneur", 198, 180, 180), // 90, 130, 80, 65, 85, 55
        new PokemonInfo(69, "Bellsprout", "Chétiflor", 158, 78, 100), // 50, 75, 35, 70, 30, 40
        new PokemonInfo(70, "Weepinbell", "Boustiflor", 190, 110, 130), // 65, 90, 50, 85, 45, 55
        new PokemonInfo(71, "Victreebel", "Empiflor", 222, 152, 160), // 80, 105, 65, 100, 70, 70
        new PokemonInfo(72, "Tentacool", "Tentacool", 106, 136, 80), // 40, 40, 35, 50, 100, 70
        new PokemonInfo(73, "Tentacruel", "Tentacruel", 170, 196, 160), // 80, 70, 65, 80, 120, 100
        new PokemonInfo(74, "Geodude", "Racaillou", 106, 118, 80), // 40, 80, 100, 30, 30, 20
        new PokemonInfo(75, "Graveler", "Gravalanch", 142, 156, 110), // 55, 95, 115, 45, 45, 35
        new PokemonInfo(76, "Golem", "Grolem", 176, 198, 160), // 80, 120, 130, 55, 65, 45
        new PokemonInfo(77, "Ponyta", "Ponyta", 168, 138, 100), // 50, 85, 55, 65, 65, 90
        new PokemonInfo(78, "Rapidash", "Galopa", 200, 170, 130), // 65, 100, 70, 80, 80, 105
        new PokemonInfo(79, "Slowpoke", "Ramoloss", 110, 110, 180), // 90, 65, 65, 40, 40, 15
        new PokemonInfo(80, "Slowbro", "Flagadoss", 184, 198, 190), // 95, 75, 110, 100, 80, 30
        new PokemonInfo(81, "Magnemite", "Magnéti", 128, 138, 50), // 25, 35, 70, 95, 55, 45
        new PokemonInfo(82, "Magneton", "Magnéton", 186, 180, 100), // 50, 60, 95, 120, 70, 70
        new PokemonInfo(83, "Farfetch'd", "Canarticho", 138, 132, 104), // 52, 65, 55, 58, 62, 60
        new PokemonInfo(84, "Doduo", "Doduo", 126, 96, 70), // 35, 85, 45, 35, 35, 75
        new PokemonInfo(85, "Dodrio", "Dodrio", 182, 150, 120), // 60, 110, 70, 60, 60, 100
        new PokemonInfo(86, "Seel", "Otaria", 104, 138, 130), // 65, 45, 55, 45, 70, 45
        new PokemonInfo(87, "Dewgong", "Lamantine", 156, 192, 180), // 90, 70, 80, 70, 95, 70
        new PokemonInfo(88, "Grimer", "Tadmorv", 124, 110, 160), // 80, 80, 50, 40, 50, 25
        new PokemonInfo(89, "Muk", "Grotadmorv", 180, 188, 210), // 105, 105, 75, 65, 100, 50
        new PokemonInfo(90, "Shellder", "Kokiyas", 120, 112, 60), // 30, 65, 100, 45, 25, 40
        new PokemonInfo(91, "Cloyster", "Crustabri", 196, 196, 100), // 50, 95, 180, 85, 45, 70
        new PokemonInfo(92, "Gastly", "Fantominus", 136, 82, 60), // 30, 35, 30, 100, 35, 80
        new PokemonInfo(93, "Haunter", "Spectrum", 172, 118, 90), // 45, 50, 45, 115, 55, 95
        new PokemonInfo(94, "Gengar", "Ectoplasma", 204, 156, 120), // 60, 65, 60, 130, 75, 110
        new PokemonInfo(95, "Onix", "Onix", 90, 186, 70), // 35, 45, 160, 30, 45, 70
        new PokemonInfo(96, "Drowzee", "Soporifik", 104, 140, 120), // 60, 48, 45, 43, 90, 42
        new PokemonInfo(97, "Hypno", "Hypnomade", 162, 196, 170), // 85, 73, 70, 73, 115, 67
        new PokemonInfo(98, "Krabby", "Krabby", 116, 110, 60), // 30, 105, 90, 25, 25, 50
        new PokemonInfo(99, "Kingler", "Krabboss", 178, 168, 110), // 55, 130, 115, 50, 50, 75
        new PokemonInfo(100, "Voltorb", "Voltorbe", 102, 124, 80), // 40, 30, 50, 55, 55, 100
        new PokemonInfo(101, "Electrode", "Électrode", 150, 174, 120), // 60, 50, 70, 80, 80, 140
        new PokemonInfo(102, "Exeggcute", "Neouneouf", 110, 132, 120), // 60, 40, 80, 60, 45, 40
        new PokemonInfo(103, "Exeggutor", "Noadkoko", 232, 164, 190), // 95, 95, 85, 125, 65, 55
        new PokemonInfo(104, "Cubone", "Osselait", 102, 150, 100), // 50, 50, 95, 40, 50, 35
        new PokemonInfo(105, "Marowak", "Ossatueur", 140, 202, 120), // 60, 80, 110, 50, 80, 45
        new PokemonInfo(106, "Hitmonlee", "Kicklee", 148, 172, 100), // 50, 120, 53, 35, 110, 87
        new PokemonInfo(107, "Hitmonchan", "Tygnon", 138, 204, 100), // 50, 105, 79, 35, 110, 76
        new PokemonInfo(108, "Lickitung", "Excelangue", 126, 160, 180), // 90, 55, 75, 60, 75, 30
        new PokemonInfo(109, "Koffing", "Smogo", 136, 142, 80), // 40, 65, 95, 60, 45, 35
        new PokemonInfo(110, "Weezing", "Smogogo", 190, 198, 130), // 65, 90, 120, 85, 70, 60
        new PokemonInfo(111, "Rhyhorn", "Rhinocorne", 110, 116, 160), // 80, 85, 95, 30, 30, 25
        new PokemonInfo(112, "Rhydon", "Rhinoféros", 166, 160, 210), // 105, 130, 120, 45, 45, 40
        new PokemonInfo(113, "Chansey", "Leveinard", 40, 60, 500), // 250, 5, 5, 35, 105, 50
        new PokemonInfo(114, "Tangela", "Saquedeneu", 164, 152, 130), // 65, 55, 115, 100, 40, 60
        new PokemonInfo(115, "Kangaskhan", "Kangourex", 142, 178, 210), // 105, 95, 80, 40, 80, 90
        new PokemonInfo(116, "Horsea", "Hypotrempe", 122, 100, 60), // 30, 40, 70, 70, 25, 60
        new PokemonInfo(117, "Seadra", "Hypocéan", 176, 150, 110), // 55, 65, 95, 95, 45, 85
        new PokemonInfo(118, "Goldeen", "Poissirène", 112, 126, 90), // 45, 67, 60, 35, 50, 63
        new PokemonInfo(119, "Seaking", "Poissoroy", 172, 160, 160), // 80, 92, 65, 65, 80, 68
        new PokemonInfo(120, "Staryu", "Stari", 130, 128, 60), // 30, 45, 55, 70, 55, 85
        new PokemonInfo(121, "Starmie", "Staross", 194, 192, 120), // 60, 75, 85, 100, 85, 115
        new PokemonInfo(122, "Mr. Mime", "M. Mime", 154, 196, 80), // 40, 45, 65, 100, 120, 90
        new PokemonInfo(123, "Scyther", "Insécateur", 176, 180, 140), // 70, 110, 80, 55, 80, 105
        new PokemonInfo(124, "Jynx", "Lippoutou", 172, 134, 130), // 65, 50, 35, 115, 95, 95
        new PokemonInfo(125, "Electabuzz", "Élektek", 198, 160, 130), // 65, 83, 57, 95, 85, 105
        new PokemonInfo(126, "Magmar", "Magmar", 214, 158, 130), // 65, 95, 57, 100, 85, 93
        new PokemonInfo(127, "Pinsir", "Scarabrute", 184, 186, 130), // 65, 125, 100, 55, 70, 85
        new PokemonInfo(128, "Tauros", "Tauros", 148, 184, 150), // 75, 100, 95, 40, 70, 110
        new PokemonInfo(129, "Magikarp", "Magicarpe", 42, 84, 40), // 20, 10, 55, 15, 20, 80
        new PokemonInfo(130, "Gyarados", "Léviator", 192, 196, 190), // 95, 125, 79, 60, 100, 81
        new PokemonInfo(131, "Lapras", "Lokhass", 186, 190, 260), // 130, 85, 80, 85, 95, 60
        new PokemonInfo(132, "Ditto", "Métamorph", 110, 110, 96), // 48, 48, 48, 48, 48, 48
        new PokemonInfo(133, "Eevee", "Évoli", 114, 128, 110), // 55, 55, 50, 45, 65, 55
        new PokemonInfo(134, "Vaporeon", "Aquali", 186, 168, 260), // 130, 65, 60, 110, 95, 65
        new PokemonInfo(135, "Jolteon", "Voltali", 192, 174, 130), // 65, 65, 60, 110, 95, 130
        new PokemonInfo(136, "Flareon", "Pyroli", 238, 178, 130), // 65, 130, 60, 95, 110, 65
        new PokemonInfo(137, "Porygon", "Porygon", 156, 158, 130), // 65, 60, 70, 85, 75, 40
        new PokemonInfo(138, "Omanyte", "Amonita", 132, 160, 70), // 35, 40, 100, 90, 55, 35
        new PokemonInfo(139, "Omastar", "Amonistar", 180, 202, 140), // 70, 60, 125, 115, 70, 55
        new PokemonInfo(140, "Kabuto", "Kabuto", 148, 142, 60), // 30, 80, 90, 55, 45, 55
        new PokemonInfo(141, "Kabutops", "Kabutops", 190, 190, 120), // 60, 115, 105, 65, 70, 80
        new PokemonInfo(142, "Aerodactyl", "Ptéra", 182, 162, 160), // 80, 105, 65, 60, 75, 130
        new PokemonInfo(143, "Snorlax", "Ronflex", 180, 180, 320), // 160, 110, 65, 65, 110, 30
        new PokemonInfo(144, "Articuno", "Artikodin", 198, 242, 180), // 90, 85, 100, 95, 125, 85
        new PokemonInfo(145, "Zapdos", "Électhor", 232, 194, 180), // 90, 90, 85, 125, 90, 100
        new PokemonInfo(146, "Moltres", "Sulfura", 242, 194, 180), // 90, 100, 90, 125, 85, 90
        new PokemonInfo(147, "Dratini", "Minidraco", 128, 110, 82), // 41, 64, 45, 50, 50, 50
        new PokemonInfo(148, "Dragonair", "Draco", 170, 152, 122), // 61, 84, 65, 70, 70, 70
        new PokemonInfo(149, "Dragonite", "Dracolosse", 250, 212, 182), // 91, 134, 95, 100, 100, 80
        new PokemonInfo(150, "Mewtwo", "Mewtwo", 284, 202, 212), // 106, 110, 90, 154, 90, 130
        new PokemonInfo(151, "Mew", "Mew", 220, 220, 200), // 100, 100, 100, 100, 100, 100
    };
}