using Microsoft.AspNetCore.Mvc;

namespace UnitTests;

public static class ActionResultExtensions
{
    // return Ok(response) 에 대한 response 추출 확장 메서드
    public static T ExtractOkValue<T>(this ActionResult<T> actionResult)
    {        
        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        return Assert.IsType<T>(ok.Value);
    }
}

/* 반환 형식에 따른 ActionResult<T>의 구조
 * 
 * case1. return Ok();
 * ActionResult<T>
 *  └─ Result = OkResult
 *      └─ Value  = null
 * 
 * 
 * case2. return Ok(response);
 * ActionResult<T>
 *  └─ Result = OkObjectResult
 *     └─ Value = response
 * 
 * 
 * case3. return response;
 * ActionResult<T>
 *  └─ Value = response
 *      └─ Result = null
 */