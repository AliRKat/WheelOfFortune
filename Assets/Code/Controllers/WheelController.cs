using System.Collections.Generic;

public class WheelController {
    private readonly System.Random _rng = new System.Random();

    public SpinResult Spin(List<WheelSlice> slices) {
        if (slices == null || slices.Count == 0)
            return SpinResult.Empty;

        int index = _rng.Next(0, slices.Count);
        var selected = slices[index];

        return new SpinResult(selected.Kind == SliceKind.Bomb, selected.Amount, selected.Id, index);
    }
}
