# Redis

**STILL FORGET THAT YOU CAN'T MAKE TAG FROM A NUMERIC FIELD**

## Funky
- When reloading now over 7k articles...
- Afterwards starting like token counter it finds few articles then stops
- Have to drop index, remake to make it work again
- Should investigate more [from module homepage](https://github.com/RediSearch)
- Lots of time I been looking issues that are just by fixed by re-creating index...

We could program functionality that drops index after reloading articles, recreate it
and query info until it shows its 100% ready.

## Helpful commands

```bash
# Delete all keys with pattern "article:*"
./redis-cli --scan --pattern "article:*" | xargs ./redis-cli del
```

- KEYS List all keys
- FT._LIST List all search indexes
- FT.EXPLAIN -> explains query, very good for learning

## Executing commands with StackExchange.Redis
This took an hour to figure out...

```csharp
// When executing command with arguments in List
var arguments = new List<string> { "idx:article", "*", "NOCONTENT" };
var resException = await db.ExecuteAsync("FT.SEARCH", arguments);
// Will result in RedisTimeoutException

// But if you execute command with array of arguments
var resOk = await db.ExecuteAsync("FT.SEARCH", arguments.ToArray());
// All will work fine
```

## indexes
- Numeric data fields can't be made tag, index breaks and nothing is found
- When we add fields to model's/indexe's redis data has to be updated or reloaded even if json has default value
but the field is missing in redis data of key and query won't work
- When querying index and ***even when defining return column*** and that column does not exists on object
***it won't return missing columns at all*** so returned data size can change depending on what columns exists on object
- When creating index, it is usable right away but using it before it is fully created will result in partial results

## Guid/Url type of columns in index
So I had added stuff with bogus to redis and was trying to search does any article have source
url `https://rosetta.net/decentralized/human/refined-granite-shoes`. So the source field was added
to the index as text field.

Studied [tokenization](https://redis.io/docs/interact/search-and-query/advanced-concepts/escaping/) on characters
that gotta be escaped. This let me to a point where I thought it should find the one article having that source value
but always end up getting 0 results. The explain from the query looked fine:
- `"@source:https://rosetta.net/decentralized/human/refined-granite-shoes\n"`

Then I Tried the same using the GUID values and that end up with same conclusion, search looks fine:
But it won't match...
- `"@id:b1653517-25f2-4c82-87b5-f610e7fd6dbc\n"`

But when I switched the field types to tag I could match the exact value. But all could been easier if I had read the
documentation better :) But this was good practice

- TEXT - Allows full-text search queries against the value in this attribute.
- TAG - Allows ***exact-match queries***, such as categories or primary keys, against the value in this attribute. For more information, see Tag Fields.

As Text -> naah
```
FT.DROPINDEX idx:article
FT.CREATE "idx:article" ON JSON PREFIX 1 article: SCORE 1.0 SCHEMA $.id AS id TEXT WEIGHT 1.0
FT.SEARCH "idx:article" "*"
FT.SEARCH "idx:article" "@id:\"b1653517\\-25f2\\-4c82\\-87b5\\-f610e7fd6dbc\""
# explain: "@id:b1653517-25f2-4c82-87b5-f610e7fd6dbc\n"
```

As Tag -> works
```
FT.DROPINDEX idx:article
FT.CREATE "idx:article" ON JSON PREFIX 1 article: SCORE 1.0 SCHEMA $.id AS id TAG
FT.SEARCH "idx:article" "*"
FT.SEARCH "idx:article" "@id:{b1653517\\-25f2\\-4c82\\-87b5\\-f610e7fd6dbc}"
# explain: "TAG:@id {\n  b1653517-25f2-4c82-87b5-f610e7fd6dbc\n}\n"
```

So after reading documentation better and practicing yeah tag is the correct type for these.
Maybe if some point I would like to search articles where source is some specific domain I need to add it as text...

Can I add same field twice? Yes :)

```
FT.DROPINDEX idx:article
FT.CREATE "idx:article" ON JSON PREFIX 1 article: SCORE 1.0 SCHEMA $.id AS id TAG $.source as source TAG $.source as sourcetext TEXT WEIGHT 1.0

# ok... maybe not sure really as '.' means
FT.SEARCH "idx:article" "@sourcetext:rosetta.net"
# explain: "@sourcetext:INTERSECT {\n  @sourcetext:UNION {\n    @sourcetext:rosetta\n    @sourcetext:+rosetta(expanded)\n  }\n  @sourcetext:UNION {\n    @sourcetext:net\n    @sourcetext:+net(expanded)\n  }\n}\n"

# ok... correct way?
FT.SEARCH "idx:article" "@sourcetext:rosetta\.net"
# explain: "@sourcetext:INTERSECT {\n  @sourcetext:UNION {\n    @sourcetext:rosetta\n    @sourcetext:+rosetta(expanded)\n  }\n  @sourcetext:UNION {\n    @sourcetext:net\n    @sourcetext:+net(expanded)\n  }\n}\n"

# not finding
FT.SEARCH "idx:article" "@sourcetext:rosetta\\.net"
# explain: "@sourcetext:UNION {\n  @sourcetext:rosetta.net\n  @sourcetext:+rosetta.net(expanded)\n}\n"

```

## Index
Example
```
FT.CREATE idxArticle ON JSON PREFIX 1 article: SCORE 1.0 SCHEMA $.id AS id TEXT WEIGHT 1.0 $.title AS title TEXT WEIGHT 1.0 $.source AS source TEXT WEIGHT 1.0 $.published AS published TAG
```

# Random things learned along the way
- It really is impossible to run any blazor application in subfolder/virtual folder
	- Issue is that I found now way to configure the dll's generated blazor.server.js to be found
	- I exhausted myself using AI and internet for around 10 hours to find a solution
- dotnet... i wanted to use 8 but not ready so 7 it is
	- Experienced issues with 8.0 preview
	- Using .net 7.0, because tried two hours to run [sample codes](https://github.com/dotnet/blazor-samples.git)
	with the 8.0 preview template. Not sure did I break template but all worked with 7.
- redis can be integrated directly [read the documentation](https://learn.microsoft.com/en-us/aspnet/core/signalr/redis-backplane?view=aspnetcore-7.0)
	- package [Microsoft.AspNetCore.SignalR.StackExchangeRedis](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.signalr.stackexchangeredis?view=aspnetcore-7.0)
	- package kinda does same thing I had in mind to relay messages
	- still if I want to create let's say some rust app that produces data don't think I can send data directly to these channels
	  as they are generated as needed and follow own naming conventions and message formats
		- so I can send to my custom channel data from the rust app that something like the ChatRelayService then send into signal-r
		- ofc the setup using redis with signal-r I can reach all servers directly
- each signal-r hub has its own websocket connection (pre .net 4.7 there was only one connection for all hubs)
	- there is limitation on number of users single server can handle
- azure signal-r service moves the socket load from server to service (the selling point of the product)

# Compiling Redis and modules
- Using fresh version compiled today (2023-12-12), hoping the weird issues with indexes are gone

## Redis
This is easy to compile using make

## RediJSON
This is easy to compile using cargo (rust)

## RediSearch
```bash
git clone --recursive https://github.com/RediSearch/RediSearch.git
```

## Using conda environment (redis)
- python 3.10.12
- [Conda environment yaml](./redis_environment.yaml)

In conda environment running sbin/system-setup.py seems to work fine as no errors are visible.

Note! I had to `pip install conan` to get it working again.

### Issue deps/VectorSimilarity
Does not compile ok with latest GCC.

Fixed by compilation flags [PyTorch same issue](https://github.com/pytorch/FBGEMM/issues/1094)
```
export CFLAGS+=" -Wno-error=maybe-uninitialized -Wno-error=uninitialized -Wno-error=restrict"
export CXXFLAGS+=" -Wno-error=maybe-uninitialized -Wno-error=uninitialized -Wno-error=restrict"
```

But in this case modify Makefile CMAKE_FLAGS to include those and
modify CMakeLists.txt CMAKE_CXX_FLAGS to include those.

Then compilation and even unit_test works.

# Original developmer readme for RediSearch
Stolen from source codeex
---
title: "Developer notes"
linkTitle: "Developer notes"
weight: 3
description: Notes on debugging, testing and documentation
aliases:
    - /docs/stack/search/development/
---

## Developing RediSearch

Developing RediSearch features involves setting up a development environment (which can be either Linux-based or macOS-based), building the module, running tests and benchmarks, and debugging both the module and its tests.

### Cloning the git repository

Run the following command to clone the RediSearch module and its submodules:

```sh
git clone --recursive https://github.com/RediSearch/RediSearch.git
```

### Working in an isolated environment

There are several reasons to develop in an isolated environment, like keeping your workstation clean, and developing for a different Linux distribution.
The most general option for an isolated environment is a virtual machine. It's very easy to set one up using [Vagrant](https://www.vagrantup.com)).
Docker is even more agile, as it offers an almost instant solution:

```
search=$(docker run -d -it -v $PWD:/build debian:bullseye bash)
docker exec -it $search bash
```

Then, from within the container, `cd /build` and go on as usual.

In this mode, all installations remain in the scope of the Docker container.
Upon exiting the container, you can either re-invoke it with the above `docker exec` or commit the state of the container to an image and re-invoke it at a later stage:

```
docker commit $search redisearch1
docker stop $search
search=$(docker run -d -it -v $PWD:/build rediseatch1 bash)
docker exec -it $search bash
```

You can replace `debian:bullseye` with your choice of OS, with the host OS being the best choice allowing you to run the RediSearch binary on your host after it is built.

### Installing prerequisites

To build and test RediSearch you need to install several packages, depending on the underlying OS. The following OSes are supported: Ubuntu/Debian, CentOS, Fedora, and macOS.

First, enter the `RediSearch` directory and then run:

```
./sbin/setup
bash -l
```

Note that this will install various packages on your system using the native package manager and `pip`. It will invoke `sudo` on its own, prompting for permission.

If you prefer to avoid that, you can:

* Review `sbin/system-setup.py` and install packages manually.
* Use `./sbin/system-setup.py --nop` to display installation commands without executing them.
* Use an isolated environment as explained above.
* Use a Python virtual environment, as Python installations are known to be sensitive when not used in isolation:

    ```
    python3 -m virtualenv venv; . ./venv/bin/activate
    ```

### Installing Redis
As a rule of thumb, you're run the latest Redis version.

If your OS has a Redis 6.x or 7.x package, you can install it using the OS package manager.

Otherwise, you can invoke ```./deps/readies/bin/getredis```.

### Getting help

```make help``` provides a quick summary of the development features:

```
make setup         # install prerequisited (CAUTION: THIS WILL MODIFY YOUR SYSTEM)
make fetch         # download and prepare dependant modules

make build          # compile and link
  COORD=1|oss|rlec    # build coordinator (1|oss: Open Source, rlec: Enterprise)
  STATIC=1            # build as static lib
  LITE=1              # build RediSearchLight
  DEBUG=1             # build for debugging
  NO_TESTS=1          # disable unit tests
  WHY=1               # explain CMake decisions (in /tmp/cmake-why)
  FORCE=1             # Force CMake rerun (default)
  CMAKE_ARGS=...      # extra arguments to CMake
  VG=1                # build for Valgrind
  SAN=type            # build with LLVM sanitizer (type=address|memory|leak|thread)
  SLOW=1              # do not parallelize build (for diagnostics)
  GCC=1               # build with GCC (default unless Sanitizer)
  CLANG=1             # build with CLang
  STATIC_LIBSTDCXX=0  # link libstdc++ dynamically (default: 1)
make parsers       # build parsers code
make clean         # remove build artifacts
  ALL=1              # remove entire artifacts directory

make run           # run redis with RediSearch
  GDB=1              # invoke using gdb

make test          # run all tests
  COORD=1|oss|rlec   # test coordinator (1|oss: Open Source, rlec: Enterprise)
  TEST=name          # run specified test
make pytest        # run python tests (tests/pytests)
  COORD=1|oss|rlec   # test coordinator (1|oss: Open Source, rlec: Enterprise)
  TEST=name          # e.g. TEST=test:testSearch
  RLTEST_ARGS=...    # pass args to RLTest
  REJSON=1|0|get     # also load JSON module (default: 1)
  REJSON_PATH=path   # use JSON module at `path`
  EXT=1              # External (existing) environment
  GDB=1              # RLTest interactive debugging
  VG=1               # use Valgrind
  VG_LEAKS=0         # do not search leaks with Valgrind
  SAN=type           # use LLVM sanitizer (type=address|memory|leak|thread)
  ONLY_STABLE=1      # skip unstable tests
make unit-tests    # run unit tests (C and C++)
  TEST=name          # e.g. TEST=FGCTest.testRemoveLastBlock
make c_tests       # run C tests (from tests/ctests)
make cpp_tests     # run C++ tests (from tests/cpptests)
make vecsim-bench  # run VecSim micro-benchmark

make callgrind     # produce a call graph
  REDIS_ARGS="args"

make pack             # create installation packages (default: 'redisearch-oss' package)
  COORD=rlec            # pack RLEC coordinator ('redisearch' package)
  LITE=1                # pack RediSearchLight ('redisearch-light' package)

make upload-artifacts   # copy snapshot packages to S3
  OSNICK=nick             # copy snapshots for specific OSNICK
make upload-release     # copy release packages to S3

common options for upload operations:
  STAGING=1             # copy to staging lab area (for validation)
  FORCE=1               # allow operation outside CI environment
  VERBOSE=1             # show more details
  NOP=1                 # do not copy, just print commands

make docker        # build for specified platform
  OSNICK=nick        # platform to build for (default: host platform)
  TEST=1             # run tests after build
  PACK=1             # create package
  ARTIFACTS=1        # copy artifacts to host

make box           # create container with volumen mapping into /search
  OSNICK=nick        # platform spec
make sanbox        # create container with CLang Sanitizer
```

### Building from source

```make build``` will build RediSearch.

`make build COORD=oss` will build OSS RediSearch Coordinator.

`make build STATIC=1` will build as a static library.

Notes:

* Binary files are placed under `bin`, according to platform and build variant.
* RediSearch uses [CMake](https://cmake.org) as its build system. ```make build``` will invoke both CMake and the subsequent make command that's required to complete the build.

Use ```make clean``` to remove build artifacts. ```make clean ALL=1``` will remove the entire `bin` subdirectory.

#### Diagnosing the build process

`make build` will build in parallel by default.

For the purposes of build diagnosis, `make build SLOW=1 VERBOSE=1` can be used to examine compilation commands.

### Running Redis with RediSearch

The following will run ```redis``` and load the RediSearch module.

```
make run
```
You can open ```redis-cli``` in another terminal to interact with it.

### Running tests

There are several sets of unit tests:
* C tests, located in ```tests/ctests```, run by ```make c_tests```.
* C++ tests (enabled by GTest), located in ```tests/cpptests```, run by ```make cpp_tests```.
* Python tests (enabled by RLTest), located in ```tests/pytests```, run by ```make pytest```.

You can run all tests by invoking ```make test```.

A single test can be run using the ```TEST``` parameter, e.g., ```make test TEST=regex```.

### Debugging

To build for debugging (enabling symbolic information and disabling optimization), run ```make DEBUG=1```.
You can then use ```make run DEBUG=1``` to invoke ```gdb```.
In addition to the usual way to set breakpoints in ```gdb```, it is possible to use the ```BB``` macro to set a breakpoint inside the RediSearch code. It will only have an effect when running under ```gdb```.

Similarly, Python tests in a single-test mode, you can set a breakpoint by using the ```BB()``` function inside a test.
