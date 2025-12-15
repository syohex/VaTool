DOTNET_VERSION=10.0
LINUX_BIN_DIR=bin/Release/net$(DOTNET_VERSION)/linux-x64/publish
INSTALL_DIR=~/.dotnet/tools
BUILD_FLAGS=-c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true

BLOG_GEN=src/VaTool.BlogGen/$(LINUX_BIN_DIR)/VaTool.BlogGen
WIKI_GEN=src/VaTool.WikiGen/$(LINUX_BIN_DIR)/VaTool.WikiGen
PLAYWRITE_DIR=src/VaTool.BlogGen/$(LINUX_BIN_DIR)/.playwright
PLAYWRITE_SCRIPT=src/VaTool.BlogGen/$(LINUX_BIN_DIR)/$(DOTNET_VERSION)/playwright.ps1

all: $(BLOG_GEN) $(WIKI_GEN)

$(BLOG_GEN): src/VaTool.BlogGen/*.cs src/VaTool.Lib/*.cs
	dotnet publish $(BUILD_FLAGS) src/VaTool.BlogGen/VaTool.BlogGen.csproj 

$(WIKI_GEN):  src/VaTool.WikiGen/*.cs src/VaTool.Lib/*.cs
	dotnet publish $(BUILD_FLAGS) src/VaTool.WikiGen/VaTool.WikiGen.csproj 

install: $(BLOG_GEN) $(WIKI_GEN)
	install -d $(INSTALL_DIR)
	cp $(BLOG_GEN) $(INSTALL_DIR)/bloggen
	cp $(WIKI_GEN) $(INSTALL_DIR)/wikigen
	rm -rf $(INSTALL_DIR)/.playwright
	cp -r $(PLAYWRITE_DIR) $(INSTALL_DIR)

.PHONY: install-browser
install-browser:
	pwsh $(PLAYWRITE_SCRIPT) install chromium

.PHONY: clean
clean:
	dotnet clean
	find . -type d -name bin | xargs rm -rf
	find . -type d -name obj | xargs rm -rf
