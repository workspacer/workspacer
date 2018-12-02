<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
                xmlns="http://schemas.microsoft.com/wix/2006/wi"
                exclude-result-prefixes="xsl wix">

  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

  <xsl:strip-space elements="*"/>

  <xsl:key name="ToRemove"
           match="wix:Component[contains(wix:File/@Source, '.pdb') or contains(wix:File/@Source, '.xml')]"
           use="@Id" />

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="*[self::wix:Component or self::wix:ComponentRef]
                        [key('ToRemove', @Id)]" />

  <xsl:template match='wix:Component[contains(wix:File/@Source, "workspacer.exe") and not(contains(wix:File/@Source, "workspacer.exe.config"))]'> 
    <xsl:copy>
      <xsl:apply-templates select="@*" />
      <!-- Adding the Win64-attribute as we have a x64 application -->
      <xsl:attribute name="Win64">yes</xsl:attribute>

      <xsl:comment> added shortcut under Component with File that has Source with workspacer.exe </xsl:comment>
      <Shortcut 
        Id="workspacerExeShortcut" 
        Name="workspacer" 
        Directory="ApplicationProgramsFolder" 
        Advertise="yes" WorkingDirectory="INSTALLDIR">
        <Icon Id="workspacerIcon.exe" SourceFile="$(var.SourceDir)\workspacer.exe" />
      </Shortcut>
      <RemoveFolder
        Id="workspacerExeShortcut_ProgramMenuFolder_ApplicationProgramsFolder"  
        Directory="ApplicationProgramsFolder" 
        On="uninstall" />

      <!-- Now take the rest of the inner tag -->
      <xsl:apply-templates select="node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="wix:Component">
    <xsl:copy>
      <xsl:apply-templates select="@*" />
      <!-- Adding the Win64-attribute as we have a x64 application -->
      <xsl:attribute name="Win64">yes</xsl:attribute>

      <!-- Now take the rest of the inner tag -->
      <xsl:apply-templates select="node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
