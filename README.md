# GoOnTap

Go on Tap is an Android application to calculate your Pokemons IVs directly from Pokemon GO.

It uses the assist API from Android 6.0 to replace Google Now on Tap. Once set up, you can long press home button to trigger recognition and Go on Tap will instantly show your Pokemon stats.

This app is fully open source under MIT license, and released without any ad. You can download it directly from the Play Store following this link : [https://play.google.com/store/apps/details?id=net.thedju.GoOnTap](https://play.google.com/store/apps/details?id=net.thedju.GoOnTap)

<p align="center">
    <img width="48" src="https://raw.githubusercontent.com/jbatonnet/goontap/master/Data/Logo-512.png" />
</p>

## Structure

- **Data** : Resources, logos, screenshot samples and character recognition database
- **GoOnTap.Android** : The Android application, using Xamarin
- **GoOnTap.Test** : A simple console application to run the recognition algorithm on screenshot samples.

## Development

Go on Tap uses assist API from Android 6.0. There is not a lot of documentation available, but here are my main sources :
- Official (very) short documentation [https://developer.android.com/training/articles/assistant.html#implementing_your_own_assistant](https://developer.android.com/training/articles/assistant.html#implementing_your_own_assistant)
- A good java sample : [http://crypto.nknu.edu.tw/AOSP/Android6/cts/tests/tests/assist/service/src/android/voiceinteraction/service/](http://crypto.nknu.edu.tw/AOSP/Android6/cts/tests/tests/assist/service/src/android/voiceinteraction/service/)
- And now my app in C# :)

When triggered, this API sends a screenshot to the application. Go on Tap can now visually recognize UI elements in the following order
- Pokemon display zone
- CP arc/angle
- Character matching for Pokemon CP, name, HP and candy name against a known characters database

Once all these elements are recognized, we can estimate our Pokemon stats :
- Using arc angle and player level, we can calculate the Pokemon level
- Pokemon might have a custom name, so I try to use candy name, recognized CP and HP to find the best match.
- Once we know which Pokemon is shown, we can compute its IVs possibilities

<p align="center">
    <img width="48" src="https://raw.githubusercontent.com/jbatonnet/goontap/master/Data/Logo-512.png" />
</p>

## Limitations

The formulas used to compute Pokemon level are taken from The Silph Road javascript calculator. It is not perfect, especially for high level Pokemons.

## Background

I used C# 6 and Xamarin to build this application. These components are available for free in Visual Studio Community 2015.

If you want to build this application by yourself, just install Visual Studio 2015 with Xamarin components. You will be able to start GoOnTap.Android project directly on your device/emulator.

This project is an attempt to learn how to build, distribute and maintain an Android application using technologies I love.

## Contributions

Thank to [Googulator](https://github.com/Googulator) for his contributions.