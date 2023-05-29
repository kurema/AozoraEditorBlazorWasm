<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
	xmlns:l="https://github.com/kurema/AozoraEditorBlazorWasm/blob/master/AozoraEditor/AozoraEditorSharedUI/License/License.xsd"
>
	<xsl:output method="html" indent="yes"/>

	<xsl:template match="/">
		<html>
			<head>
				<meta charset="utf-8" />
				<title>サードパーティー通知</title>
				<meta name="viewport" content="width=device-width,initial-scale=1.0,minimum-scale=1.0,maximum-scale=1.0,user-scalable=yes" />
			</head>
			<body>
				<style>
					body {
					margin: 2% 4%;
					}

					.entry {
					}

					.entry > details.license {
					white-space: pre;
					}
				</style>
				<xsl:for-each select="l:licenses/l:entry">
					<div class="entry">
						<h1 class="entry_name">
							<xsl:value-of select="l:name"/>
						</h1>
						<p class="comment">
							<xsl:value-of select="l:info"/>
						</p>
						<details class="license">
							<summary class="summary">
								<xsl:value-of select="l:summary"/>
							</summary>
							<xsl:value-of select="l:license"/>
						</details>
					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
