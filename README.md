Meerkat Security Hmac
=====================

The [Meerkat.Security.Hmac](https://www.nuget.org/packages/Meerkat.Security.Hmac/) library is an implementation of HMAC authentication for ASP.NET MVC

[![NuGet](https://img.shields.io/nuget/v/Meerkat.Security.Hmac.svg)](https://www.nuget.org/packages/Meerkat.Security.Hmac/) 
[![Build status](https://ci.appveyor.com/api/projects/status/3a3t90hixblfii3p/branch/master?svg=true)](https://ci.appveyor.com/project/PaulHatcher/Meerkat.Security.Hmac/branch/master)


Welcome to contributions from anyone.

You can see the version history [here](RELEASE_NOTES.md).

## Build the project
* Windows: Run *build.cmd*

I have my tools in C:\Tools so I use *build.cmd Default tools=C:\Tools encoding=UTF-8*

## Library License

The library is available under the [MIT License](http://en.wikipedia.org/wiki/MIT_License), for more information see the [License file][1] in the GitHub repository.

 [1]: https://github.com/phatcher/Meerkat.Security.Hmac/blob/master/License.md

## Getting Started

HMAC is a protocol that proves that the messsage comes from a trusted sender - assuming of course that the signing keys have not been compromised (more later on that).

The flow from the client looks like this..

1. Retrieve your client id and secret from where you are keeping it
2. Produce a message representation 
3. Hash it with the secret using the HMAC algorithm
4. Send the message

On the server side we get...

1. Check if the message is HMAC authenticated, if not ignore it
2. Check if the message is within time e.g. is the sent time +/- 5 minutes of the server time, allows for some clock skew between client and server
3. Check the MD5 hash of the content
4. Retrieve the client id from the message and use that to locate the appropriate secret
5. Produce a message representation
6. Hash it with the secret using the HMAC algorithm
7. Check that the hashs match, if not fail
8. Check if we've seen this hash before, if so fail - this handles replay attacks

There are a number of components that need wiring together for this to work, so I'm using Unity in the examples to provide IoC/DI services, but you can easily use your preferred 
container to do the same.

So to use the library you must secure both the server and the clients that will communicate with it, let's start with the client...

First we need to wire up the basic components

'
    public static void RegisterTypes(IUnityContainer container)
    {
        container.RegisterType<ISecretStore, SecretStore>();
        container.RegisterType<IMessageRepresentationBuilder, MessageRepresentationBuilder>();
        container.RegisterType<ISignatureCalculator, HmacSignatureCalculator>();

        container.RegisterType<HmacSigningHandler>();
    }
'

Next, when we create a HttpClient we need to wire up the appropriate handlers so there is a utility function in the form class as follows

'
    public HttpMessageHandler CreateHandler()
    {
        var client = new HttpClientHandler();

        var hmac = Container.Resolve<HmacSigningHandler>();
        hmac.InnerHandler = client;

        var md5 = new RequestContentMd5Handler
        {
            InnerHandler = hmac
        };

        return md5;
    }
'

Note the order with the MD5 hash happening before the HMAC and the standard HttpClientHandler being last to do the actual transmission. You can add your own handlers as needed but 
the relative order of the these three handlers should remain the same.