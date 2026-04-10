// MODELS = the shape of data that flows in and out of our API.


namespace FunctionPointAPI.Models;


public class FunctionTypeCount
{
    public int Simple { get; set; }
    public int Average { get; set; }
    public int Complex { get; set; }
}

public class UfpRequest
{
    public FunctionTypeCount ExternalInputs { get; set; } = new();
    public FunctionTypeCount ExternalOutputs { get; set; } = new();
    public FunctionTypeCount ExternalInquiries { get; set; } = new();
    public FunctionTypeCount InternalLogicalFiles { get; set; } = new();
    public FunctionTypeCount ExternalInterfaceFiles { get; set; } = new();
}


public class TcfRequest
{
   
    public int? Di { get; set; }

  
    public List<int>? Attributes { get; set; }
}



public class FpRequest
{
    public double Ufp { get; set; }
    public double Tcf { get; set; }
}

public class LocRequest
{
    public double Fp { get; set; }
    public string Language { get; set; } = string.Empty;
}




public class UfpResponse
{
    public int TotalUfp { get; set; }

   
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
