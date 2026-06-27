.PHONY: format build test coverage pack clean

format:
	dotnet format

build:
	dotnet build

test:
	dotnet test

coverage:
	dotnet test --collect:"XPlat Code Coverage" --results-directory:./TestResults
	dotnet tool install -g dotnet-reportgenerator-globaltool && reportgenerator -reports:./TestResults/**/coverage.cobertura.xml -targetdir:./CoverageReport -reporttypes:Html || true

pack:
	dotnet pack -c Release

clean:
	dotnet clean
