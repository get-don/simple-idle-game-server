using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace GameServer.Utils;

// swagger 페이지의 Authorize 관련 코드.
public class OpenApiSessionTokenSecurityTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        // securitySchemes 등록
        document.Components.SecuritySchemes["SessionToken"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "X-Session-Token",
            Description = "세션 인증 토큰"
        };

        // 전역 security requirement 등록 (모든 API에 적용)
        document.SecurityRequirements ??= new List<OpenApiSecurityRequirement>();
        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            // Scheme 참조
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "SessionToken"
                }
            }] = Array.Empty<string>()
        });

        return Task.CompletedTask;
    }
}
