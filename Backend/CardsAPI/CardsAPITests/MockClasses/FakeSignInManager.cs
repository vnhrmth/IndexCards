﻿using CardsAPITests.MockClasses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

public class FakeSignInManager : SignInManager<IdentityUser>
{
	public FakeSignInManager()
		: base(new FakeUserManager(),
		new Mock<IHttpContextAccessor>().Object,
		new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
		new Mock<IOptions<IdentityOptions>>().Object,
		new Mock<ILogger<SignInManager<IdentityUser>>>().Object,
		new Mock<IAuthenticationSchemeProvider>().Object)
	{ }
}
