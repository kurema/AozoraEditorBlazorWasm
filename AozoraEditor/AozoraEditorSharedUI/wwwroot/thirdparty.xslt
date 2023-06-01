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
					max-width: 970px;
					margin-left:auto;
					margin-right:auto;
					}

					.entry {
					display: grid;
					grid-template-columns: 1fr auto;
					grid-template-rows: auto auto auto;
					margin-bottom: 20px;
					box-shadow: #8888 0 2px 10px;
					border:1px solid #ddd;
					align-items: center;
					}

					.entry > .top_background{
					grid-row:1/2;
					grid-column:1/3;
					background:#f5f5f5;
					height: 100%;
					}

					.entry > .repository_link{
					grid-row:1/2;
					grid-column:2/3;
					background: white;
					border:1px solid #ddd;
					text-decoration: none;
					padding:5px;
					margin:10px;
					}

					.entry > .entry_name{
					grid-row:1/2;
					grid-column:1/2;
					margin-left:10px;
					margin-top:10px;
					}

					.entry > .comment{
					grid-row:2/3;
					grid-column:1/3;
					margin-left:10px;
					}

					.entry > details.license {
					white-space: pre;
					grid-row:3/4;
					grid-column:1/3;
					<!--background:#f5f5f5;-->
					border-top:1px solid #ddd;
					padding:10px;
					}

					.entry > details.license > summary {
					display: grid;
					grid-template-columns: 1fr auto;
					user-select:none;
					}

					.entry > details.license > summary::-webkit-details-marker {
					display: none;
					}

					.entry > details.license > summary > .summary_text {
					}

					.entry > details.license > summary > .icon {
					transition: transform 0.4s;
					}

					.entry > details.license[open] > summary > .icon {
					transform: rotate(180deg);
					}

				</style>
				<h1>サードパーティーライセンス</h1>
				<p>このアプリには以下の成果物が含まれています。</p>
				<xsl:for-each select="l:licenses/l:entry">
					<div class="entry">
						<div class="top_background" />
						<h2 class="entry_name">
							<xsl:value-of select="l:name"/>
						</h2>
						<a target="_blank" class="repository_link">
							<xsl:attribute name="href">
								<xsl:value-of select="@repository" />
							</xsl:attribute>
							🔗
						</a>
						<p class="comment">
							<xsl:value-of select="l:info"/>
						</p>
						<details class="license">
							<summary class="summary">
								<span class="summary_text"><xsl:value-of select="l:summary"/></span><span class="icon">＞</span>
							</summary>
							<xsl:value-of select="l:license"/>
						</details>
					</div>
				</xsl:for-each>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
