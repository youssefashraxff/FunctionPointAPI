// MODELS = the shape of data that flows in and out of our API.
// Think of them like TypeScript interfaces but in C#.

namespace FunctionPointAPI.Models;

// ──────────────────────────────────────────────
// REQUEST MODELS (what Angular sends TO our API)
// ──────────────────────────────────────────────

/// <summary>
/// Each function type (e.g. External Input) has a count for each complexity level.
/// Example: you might have 2 simple External Inputs, 1 average, and 0 complex.
/// </summary>
public class FunctionTypeCount
{
    public int Simple { get; set; }
    public int Average { get; set; }
    public int Complex { get; set; }
}

/// <summary>
/// The request body for calculating UFP.
/// Angular will send JSON like:
/// {
///   "externalInputs": { "simple": 2, "average": 1, "complex": 0 },
///   "externalOutputs": { "simple": 1, "average": 0, "complex": 1 },
///   ...
/// }
/// </summary>
public class UfpRequest
{
    public FunctionTypeCount ExternalInputs { get; set; } = new();
    public FunctionTypeCount ExternalOutputs { get; set; } = new();
    public FunctionTypeCount ExternalInquiries { get; set; } = new();
    public FunctionTypeCount InternalLogicalFiles { get; set; } = new();
    public FunctionTypeCount ExternalInterfaceFiles { get; set; } = new();
}

/// <summary>
/// The request body for calculating TCF.
/// Supports two modes:
///   Mode 1 - Direct DI input: { "di": 42 }
///   Mode 2 - 14 attributes:   { "attributes": [3, 2, 5, 0, ...] }
/// If both are provided, attributes take priority.
/// </summary>
public class TcfRequest
{
    // Mode 1: user enters DI directly
    public int? Di { get; set; }

    // Mode 2: user rates each of the 14 GSC attributes (0-5 each)
    public List<int>? Attributes { get; set; }
}

/// <summary>
/// Request for calculating FP = UFP * TCF
/// </summary>
public class FpRequest
{
    public double Ufp { get; set; }
    public double Tcf { get; set; }
}

/// <summary>
/// Request for calculating LOC = AVC * FP
/// </summary>
public class LocRequest
{
    public double Fp { get; set; }
    public string Language { get; set; } = string.Empty;
}


// ──────────────────────────────────────────────
// RESPONSE MODELS (what our API sends BACK to Angular)
// ──────────────────────────────────────────────

public class UfpResponse
{
    public int TotalUfp { get; set; }

    // Breakdown so the UI can show how each type contributed
    public Dictionary<string, int> Breakdown { get; set; } = [];
}

public class TcfResponse
{
    public int Di { get; set; }
    public double Tcf { get; set; }
}

public class FpResponse
{
    public double Fp { get; set; }
}

public class LocResponse
{
    public double Loc { get; set; }
    public string Language { get; set; } = string.Empty;
    public int Avc { get; set; }
}
