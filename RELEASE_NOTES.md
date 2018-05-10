### 3.0.4 (2018-05-10)
* Full support for netstandard20

### 3.0.3 (2018-04-14)
* Update dependencies to latest, some support for netstandard20

### 3.0.2 (2017-10-24)
* Use ConfigurAwait(false) on MD5 hash methods
* Change signature validation errors from Debug to Info, easier production logging

### 3.0.1 (2017-02-19)
* Introduce and prefer middleware using IDependencyResolver
* Introduced CachingSecretRepositorying using Meerkat.Caching

#### 3.0.0 (2017-02-04)
* Breaking change - changing namespaces to make it easier to filter on logging
* Change from Common.Logging to Meerkat.Logging, unifies logging framework across libraries.
 
#### 2.1.1 (2016-12-10)
* Fix DateTime.Kind problem by changing header date setting to DateTimeOffset.UtcNow.

#### 2.1 (2016-11-19)
* Handle content headers when converting IOwinRequest to HttpReuest, now log unhandled headers but don't error
* Improve error messages from Owin component, now same behaviour as HmacAuthorizeAttribute

#### 2.0.0 (2016-11-17)
* Introduced Owin middleware component, HmacAuthorizeAttribute is invoked too late to amend the claims using ClaimsTransformer in the Owin pipeline.
* Breaking change - Changed interface of IHmacAuthenticator to return ClaimsIdentity, simplifies Owin integration.
* Breaking change - Introduce SignatureValidator.ClockDrift property to  to split cache duration from client/server clock drift.

#### 1.2.2 (2016-11-04)
* Change date format in message representation to RFC 1123, simplifies implementation for other languages as message date header is already in this format.

#### 1.2.1 (2016-11-03)
* Fix incorrect files in WebApi NuGet package

#### 1.2 (2016-11-02)
* Extend CustomHeadersRepresentation to handle multiple headers and also multiple values in a single header
* Utility function to help for Claims for from request headers
 
#### 1.1.1 (2016-09-23)
* Introduce Meerkat.Security.Hmac.WebApi with HmacAuthorizeAttribute to authorize WebApi controllers

#### 1.1 (2016-09-22)
* Introduce IRequestClaimsProvider to provide Claims once the signature has been validated

#### 1.0.3 (2016-08-18)
* Change to use signed version of Meerkat.Caching

#### 1.0.2 (2016-05-18)
* Change to using Meerkat.Caching
* Minor internal refactoring

#### 1.0.1 (2016-04-19)
* More inline documentation
* Remove test assembly from nuspec and add documentation file

#### 1.0.0 (2016-04-17)
* First release