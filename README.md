# BreakInfinity.cs

A C# port of [break_infinity.js](https://github.com/Patashu/break_infinity.js), but for slightly bigger numbers than the original (up to approximately `1e1e308`).

## Usage

Just add the `BigDouble.cs` file to your project and use the `BigDouble` type in your code (it is defined in the `BreakInfinity` namespace).
If you use Unity, also add the `BigDoubleDrawer.cs` file to your project to have `BigDouble` variables displayed in a more convenient format in the properties panel.

## Notes

This requires at least C# 9, which means at least .NET 5 or Unity 2021.2 at minimum.

## Credits

[Patashu](https://github.com/Patashu): For the great original JavaScript/TypeScript library.