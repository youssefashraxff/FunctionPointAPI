using Microsoft.AspNetCore.Mvc;
using FunctionPointAPI.Models;

namespace FunctionPointAPI.Controllers;


[ApiController]
//  all routes start with /api/functionpoint

[Route("api/[controller]")]
public class FunctionPointController : ControllerBase
{
   
    private static readonly Dictionary<string, int[]> ComplexityWeights = new()
    {
        { "ExternalInputs",         new[] { 3, 4, 6 } },
        { "ExternalOutputs",        new[] { 4, 5, 7 } },
        { "ExternalInquiries",      new[] { 3, 4, 6 } },
        { "InternalLogicalFiles",   new[] { 7, 10, 15 } },
        { "ExternalInterfaceFiles", new[] { 5, 7, 10 } }
    };

    
    private static readonly string[] GscAttributes =
    {
        "Data Communications",
        "Distributed Data Processing",
        "Performance",
        "Heavily Used Configuration",
        "Transaction Rate",
        "Online Data Entry",
        "End-User Efficiency",
        "Online Update",
        "Complex Processing",
        "Reusability",
        "Installation Ease",
        "Operational Ease",
        "Multiple Sites",
        "Facilitate Change"
    };

    
    private static readonly Dictionary<string, int> AvcTable = new()
    {
        {"No Language",1},
        { "Assembly Language", 320 },
        { "C", 128 },
        { "COBOL/Fortran", 105 },
        { "Pascal", 90 },
        { "Ada", 70 },
        { "C++", 64 },
        { "Visual Basic", 32 },
        { "Object-Oriented Languages", 30 },
        { "Smalltalk", 22 },
        { "Code Generators (PowerBuilder)", 15 },
        { "SQL/Oracle", 12 },
        { "Spreadsheets", 6 },
        { "Graphical Languages (Icons)", 4 }
    };


   
    // ENDPOINTS
  

 
    [HttpPost("ufp")]
    public ActionResult<UfpResponse> CalculateUfp([FromBody] UfpRequest request)
    {
       
        var functionTypes = new Dictionary<string, FunctionTypeCount>
        {
            { "ExternalInputs", request.ExternalInputs },
            { "ExternalOutputs", request.ExternalOutputs },
            { "ExternalInquiries", request.ExternalInquiries },
            { "InternalLogicalFiles", request.InternalLogicalFiles },
            { "ExternalInterfaceFiles", request.ExternalInterfaceFiles }
        };

        int totalUfp = 0;
        var breakdown = new Dictionary<string, int>();

        foreach (var (typeName, counts) in functionTypes)
        {
           
            int[] weights = ComplexityWeights[typeName];

          
            int typeUfp = (counts.Simple * weights[0])
                        + (counts.Average * weights[1])
                        + (counts.Complex * weights[2]);

            breakdown[typeName] = typeUfp;
            totalUfp += typeUfp;
        }

        return Ok(new UfpResponse
        {
            TotalUfp = totalUfp,
            Breakdown = breakdown
        });
    }


    [HttpPost("tcf")]
    public ActionResult<TcfResponse> CalculateTcf([FromBody] TcfRequest request)
    {
        int di;

        if (request.Attributes != null && request.Attributes.Count == 14)
        {
           
            if (request.Attributes.Any(a => a < 0 || a > 5))
            {
                return BadRequest("Each attribute must be between 0 and 5.");
            }

            di = request.Attributes.Sum();
        }
        else if (request.Di.HasValue)
        {
          
            di = request.Di.Value;
        }
        else
        {
            return BadRequest("Provide either 'di' as an integer or 'attributes' as an array of 14 integers (0-5).");
        }

        double tcf = 0.65 + 0.01 * di;

        return Ok(new TcfResponse
        {
            Di = di,
            Tcf = Math.Round(tcf, 4)
        });
    }



    [HttpPost("fp")]
    public ActionResult<FpResponse> CalculateFp([FromBody] FpRequest request)
    {
        double fp = request.Ufp * request.Tcf;

        return Ok(new FpResponse
        {
            Fp = Math.Round(fp, 2)
        });
    }


 
    [HttpPost("loc")]
    public ActionResult<LocResponse> CalculateLoc([FromBody] LocRequest request)
    {
        if (!AvcTable.ContainsKey(request.Language))
        {
            return BadRequest($"Unknown language: '{request.Language}'. Use GET /api/functionpoint/languages to see available options.");
        }

        int avc = AvcTable[request.Language];
        double loc = avc * request.Fp;

        return Ok(new LocResponse
        {
            Loc = Math.Round(loc, 2),
            Language = request.Language,
            Avc = avc
        });
    }


   
    [HttpPost("size")]
    public ActionResult<ProjectSizeResponse> CalculateProjectSize([FromBody] ProjectSizeRequest request)
    {
        if (request.Fp <= 0)
            return BadRequest("Fp must be greater than 0.");
        if (request.Loc < 0)
            return BadRequest("Loc must be non-negative.");

        double locPerFp = Math.Round(request.Loc / request.Fp, 2);

        string sizeCategory = request.Fp switch
        {
            < 50    => "Extra Small",
            < 150   => "Small",
            < 500   => "Medium",
            < 1500  => "Large",
            _       => "Extra Large"
        };

        // Lower LOC/FP = higher-level language = better productivity
        string productivityRating = locPerFp switch
        {
            <= 10  => "Very High",
            <= 30  => "High",
            <= 64  => "Average",
            <= 128 => "Low",
            _      => "Very Low"
        };

        return Ok(new ProjectSizeResponse
        {
            Fp = Math.Round(request.Fp, 2),
            Loc = Math.Round(request.Loc, 2),
            LocPerFp = locPerFp,
            SizeCategory = sizeCategory,
            ProductivityRating = productivityRating
        });
    }


    [HttpGet("languages")]
    public ActionResult<Dictionary<string, int>> GetLanguages()
    {
        return Ok(AvcTable);
    }



    [HttpGet("gsc-attributes")]
    public ActionResult<string[]> GetGscAttributes()
    {
        return Ok(GscAttributes);
    }
}
