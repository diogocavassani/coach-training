import { routes } from './app.routes';

describe('app routes', () => {
  it('lazy loads the public pages and protected dashboard pages', () => {
    const landingRoute = routes.find((route) => route.path === '');
    const loginRoute = routes.find((route) => route.path === 'login');
    const dashboardRoute = routes.find((route) => route.path === 'dashboard');

    expect(landingRoute?.loadComponent).toEqual(jasmine.any(Function));
    expect(loginRoute?.loadComponent).toEqual(jasmine.any(Function));
    expect(dashboardRoute?.loadComponent).toEqual(jasmine.any(Function));

    const childPaths = dashboardRoute?.children?.map((route) => route.path) ?? [];

    expect(childPaths).toContain('');
    expect(childPaths).toContain('alunos');
    expect(childPaths).toContain('alunos/novo');
    expect(childPaths).toContain('alunos/:id');
    expect(childPaths).toContain('treinos/novo');

    dashboardRoute?.children?.forEach((childRoute) => {
      expect(childRoute.loadComponent).toEqual(jasmine.any(Function));
    });
  });
});
