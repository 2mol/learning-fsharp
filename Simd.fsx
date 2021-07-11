open System.Numerics

printfn $"SIMD support: {Vector.IsHardwareAccelerated}"

let m1 =
  Matrix4x4(
    1.1f, 1.2f, 1.3f, 1.4f,
    2.1f, 2.2f, 3.3f, 4.4f,
    3.1f, 3.2f, 3.3f, 3.4f,
    4.1f, 4.2f, 4.3f, 4.4f
  )

let m2 = Matrix4x4.Transpose(m1);
let mResult = Matrix4x4.Multiply(m1, m2);

printfn $"Result: {mResult}"
