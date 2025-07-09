import { themes as prismThemes } from 'prism-react-renderer';

export default {
  title: 'PipeForge',
  tagline: 'Composable pipelines for .NET',
  url: 'https://scottoffen.github.io',
  baseUrl: '/pipeforge/',
  onBrokenLinks: 'warn',
  onBrokenMarkdownLinks: 'warn',
  favicon: 'img/favicon.ico',
  trailingSlash: false,

  organizationName: 'scottoffen', // GitHub username
  projectName: 'pipeforge',       // Repo name

  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  // This enables compatibility with Docusaurus v4 (future-proof)
  future: {
    v4: true,
  },

  presets: [
    [
      'classic',
      {
        docs: {
          routeBasePath: '/', // Serve docs at the root (no /docs prefix)
          sidebarPath: './sidebars.js',
          editUrl: 'https://github.com/scottoffen/pipeforge/edit/main/docs/',
        },
        blog: false, // Disable blog
        theme: {
          customCss: './src/css/custom.css',
        },
      },
    ],
  ],

  themeConfig: {
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
          items: [
            {
              label: 'Getting Started',
              to: '/',
            },
          ],
        },
        {
          title: 'Community',
          items: [
            {
              label: 'Discussions',
              href: 'https://github.com/scottoffen/pipeforge/discussions',
            },
            {
              label: 'Stack Overflow',
              href: 'https://stackoverflow.com/questions/tagged/pipeforge',
            },
          ],
        },
        {
          title: 'Project',
          items: [
            {
              label: 'Contributing Guide',
              href: 'https://github.com/scottoffen/pipeforge/blob/main/CONTRIBUTING.md',
            },
            {
              label: 'Code of Conduct',
              href: 'https://github.com/scottoffen/pipeforge/blob/main/CODE_OF_CONDUCT.md',
            },
            {
              label: 'GitHub',
              href: 'https://github.com/scottoffen/pipeforge',
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} PipeForge`,
    },
    prism: {
      theme: prismThemes.github,
      darkTheme: prismThemes.dracula,
    },
    titleDelimiter: '|',
    titleTemplate: 'PipeForge | %s'
  },
};
