// @ts-check
// `@type` JSDoc annotations allow editor autocompletion and type checking
// (when paired with `@ts-check`).
// There are various equivalent ways to declare your Docusaurus config.
// See: https://docusaurus.io/docs/api/docusaurus-config

import {themes as prismThemes} from 'prism-react-renderer';

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'PipeForge',
  tagline: 'Composable pipelines for .NET',
  favicon: 'img/favicon.ico',

  // Future flags, see https://docusaurus.io/docs/api/docusaurus-config#future
  future: {
    v4: true, // Improve compatibility with the upcoming Docusaurus v4
  },

  // Set the production url of your site here
  url: 'https://scottoffen.github.io',
  // Set the /<baseUrl>/ pathname under which your site is served
  // For GitHub pages deployment, it is often '/<projectName>/'
  baseUrl: '/pipeforge/',

  // GitHub pages deployment config.
  // If you aren't using GitHub pages, you don't need these.
  organizationName: 'scottoffen', // Usually your GitHub org/user name.
  projectName: 'pipeforge', // Usually your repo name.

  onBrokenLinks: 'throw',

  // Even if you don't use internationalization, you can use this field to set
  // useful metadata like html lang. For example, if your site is Chinese, you
  // may want to replace "en" with "zh-Hans".
  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          routeBasePath: '/',
          sidebarPath: './sidebars.js',
          editUrl:
            'https://github.com/scottoffen/pipeforge/edit/main/docs/',
        },
        blog: false,
        theme: {
          customCss: './src/css/custom.css',
        },
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      // Replace with your project's social card
      // image: 'img/docusaurus-social-card.jpg',
      colorMode: {
        respectPrefersColorScheme: true,
      },
      navbar: {
        title: 'PipeForge',
        logo: {
          alt: 'PipeForge Logo',
          src: 'img/logo.svg',
        },
        items: [
          {
            type: 'docSidebar',
            sidebarId: 'docsSidebar',
            position: 'left',
            label: 'Docs',
          },
          {
            href: 'https://github.com/scottoffen/pipeforge',
            label: 'GitHub',
            position: 'right',
          },
        ],
      },
      footer: {
        style: 'dark',
        links: [
          {
            title: 'Documentation',
            items:[
              { label: 'Getting Started', to: '/' }
            ]
          },
          {
            title: 'Community',
            items:[
              { label: 'Discussions', href: 'https://github.com/scottoffen/pipeforge/discussions' },
              { label: 'Stack Overflow', href: 'https://stackoverflow.com/questions/tagged/pipeforge' }
            ]
          },
          {
            title: 'Project',
            items:[
              { label: 'Contributing Guide', href: 'https://github.com/scottoffen/pipeforge/blob/main/.github/contributing.md' },
              { label: 'Code of Conduct', href: 'https://github.com/scottoffen/pipeforge/blob/main/.github/code_of_conduct.md' },
              { label: 'GitHub', href: 'https://github.com/scottoffen/pipeforge' }
            ]
          }
        ],
        copyright: `Copyright © ${new Date().getFullYear()} Scott Offen`,
      },
      prism: {
        theme: prismThemes.github,
        darkTheme: prismThemes.dracula,
      },
      titleDelimiter: '|',
      titleTemplate: 'PipeForge | %s',
    }),
};

export default config;
